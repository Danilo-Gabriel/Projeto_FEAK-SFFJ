package com.example.keycloak.federation;

import org.apache.commons.codec.digest.DigestUtils;
import org.keycloak.component.ComponentModel;
import org.keycloak.credential.CredentialInput;
import org.keycloak.credential.CredentialInputValidator;
import org.keycloak.credential.CredentialModel;
import org.keycloak.models.GroupModel;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.RealmModel;
import org.keycloak.models.UserCredentialModel;
import org.keycloak.models.UserModel;
import org.keycloak.storage.StorageId;
import org.keycloak.storage.UserStorageProvider;
import org.keycloak.storage.user.UserLookupProvider;
import org.keycloak.storage.user.UserQueryProvider;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Base64;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.PBEKeySpec;
import org.jboss.logging.Logger;
import java.util.stream.Stream;

public class CustomUserStorageProvider
        implements UserStorageProvider, UserLookupProvider, CredentialInputValidator, UserQueryProvider {

    private final KeycloakSession session;
    private final ComponentModel model;
    private final String jdbcUrl;
    private final String dbUser;
    private final String dbPassword;

    private static final Logger log = Logger.getLogger(CustomUserStorageProvider.class);

    public CustomUserStorageProvider(KeycloakSession session, ComponentModel model) {
        this.session = session;
        this.model = model;

        this.jdbcUrl = model.get(CustomUserStorageProviderFactory.DB_URL);
        this.dbUser = model.get(CustomUserStorageProviderFactory.DB_USER);
        this.dbPassword = model.get(CustomUserStorageProviderFactory.DB_PASSWORD);
    }

    private Connection getConnection() throws SQLException {
        return DriverManager.getConnection(jdbcUrl, dbUser, dbPassword);
    }

    @Override
    public UserModel getUserByUsername(RealmModel realm, String username) {

        String sqlUser = "SELECT id, nome_login, nome_completo FROM usuarios "
                + "WHERE nome_login = ? AND dh_exclusao IS NULL";

        try (Connection conn = getConnection();
                PreparedStatement pUser = conn.prepareStatement(sqlUser)) {

            pUser.setString(1, username);
            ResultSet rUser = pUser.executeQuery();

            if (rUser.next()) {
                String id = rUser.getString("id");
                String login = rUser.getString("nome_login");
                String nomeCompleto = rUser.getString("nome_completo");
                String firstName = (nomeCompleto == null || nomeCompleto.isBlank()) ? login : nomeCompleto.split(" ")[0];
                String email = login + "@feak.local"; // Email padrão baseado no login

                return new FederatedUserAdapter(session, realm, model, id, login, email, firstName, true, null, null, null, null, null, null);
            }

        } catch (Exception e) {
            log.error("Erro ao buscar usuário por username: " + username, e);
        }

        return null;
    }

    @Override
    public UserModel getUserByEmail(RealmModel realm, String email) {

        String sqlUser = "SELECT id, nome_login, nome_completo FROM usuarios "
                + "WHERE nome_login = ? AND dh_exclusao IS NULL";

        try (Connection conn = getConnection();
                PreparedStatement pUser = conn.prepareStatement(sqlUser)) {

            String username = (email != null && email.contains("@")) ? email.split("@")[0] : email;
            pUser.setString(1, username);
            ResultSet rUser = pUser.executeQuery();

            if (rUser.next()) {
                String id = rUser.getString("id");
                String login = rUser.getString("nome_login");
                String nomeCompleto = rUser.getString("nome_completo");
                String firstName = (nomeCompleto == null || nomeCompleto.isBlank()) ? login : nomeCompleto.split(" ")[0];

                return new FederatedUserAdapter(session, realm, model, id, login, email, firstName, true, null, null, null, null, null, null);
            }

        } catch (Exception e) {
            log.error("Erro ao buscar usuário por email: " + email, e);
        }

        return null;
    }

    @Override
    public void close() {
        // Nothing to close because each request opens/closes its own JDBC connection.
    }

    @Override
    public UserModel getUserById(RealmModel realm, String id) {

        StorageId storageId = new StorageId(id);
        String externalId = storageId.getExternalId();

        String sqlUser = "SELECT id, nome_login, nome_completo FROM usuarios "
                + "WHERE id::text = ? AND dh_exclusao IS NULL";

        try (Connection conn = getConnection();
                PreparedStatement pUser = conn.prepareStatement(sqlUser)) {

            pUser.setString(1, externalId);
            ResultSet rUser = pUser.executeQuery();

            if (rUser.next()) {
                String userId = rUser.getString("id");
                String login = rUser.getString("nome_login");
                String nomeCompleto = rUser.getString("nome_completo");
                String firstName = (nomeCompleto == null || nomeCompleto.isBlank()) ? login : nomeCompleto.split(" ")[0];
                String email = login + "@feak.local";

                return new FederatedUserAdapter(session, realm, model, userId, login, email, firstName, true, null, null, null, null, null, null);
            }
        } catch (Exception e) {
            log.error("Erro em getUserById para id: " + externalId, e);
        }

        return null;
    }

    @Override
    public boolean supportsCredentialType(String credentialType) {
        return credentialType.equals(CredentialModel.PASSWORD);
    }

    @Override
    public boolean isConfiguredFor(RealmModel realm, UserModel user, String credentialType) {
        return supportsCredentialType(credentialType);
    }

    @Override
    public boolean isValid(RealmModel realm, UserModel user, CredentialInput credentialInput) {
        if (!(credentialInput instanceof UserCredentialModel))
            return false;
        UserCredentialModel cred = (UserCredentialModel) credentialInput;
        if (cred.getValue() == null || cred.getValue().isBlank()) {
            return false;
        }

        StorageId storageId = new StorageId(user.getId());
        String id = storageId.getExternalId();

        try (Connection conn = getConnection();
            PreparedStatement stmt = conn.prepareStatement(
                    "SELECT senha FROM usuarios WHERE id::text = ? AND dh_exclusao IS NULL")) {
            stmt.setString(1, id);
            try (ResultSet rs = stmt.executeQuery()) {
                if (rs.next()) {
                    return checkPassword(cred.getValue(), rs.getString("senha"));
                }
            }
        } catch (Exception e) {
            log.error("Erro ao validar credencial para usuário: " + user.getUsername(), e);
        }
        return false;
    }

    public static boolean checkPassword(String plainPassword, String storedHash) {
        if (plainPassword == null || storedHash == null || storedHash.isBlank()) {
            return false;
        }

        if (verifyAspNetIdentityHash(plainPassword, storedHash)) {
            return true;
        }

        // Legacy fallback for old records that may still be stored as MD5.
        String hashed = DigestUtils.md5Hex(plainPassword).toUpperCase();
        return hashed.equals(storedHash);
    }

    private static boolean verifyAspNetIdentityHash(String plainPassword, String storedHash) {
        try {
            byte[] decoded = Base64.getDecoder().decode(storedHash);
            if (decoded.length == 0) {
                return false;
            }

            int formatMarker = decoded[0] & 0xFF;

            // ASP.NET Identity V2 format marker (0x00): salt(16) + subkey(32), PBKDF2-HMAC-SHA1, 1000 iterations
            if (formatMarker == 0x00) {
                if (decoded.length != 49) {
                    return false;
                }

                byte[] salt = new byte[16];
                System.arraycopy(decoded, 1, salt, 0, 16);

                byte[] expectedSubkey = new byte[32];
                System.arraycopy(decoded, 17, expectedSubkey, 0, 32);

                byte[] actualSubkey = pbkdf2(plainPassword, salt, "PBKDF2WithHmacSHA1", 1000, 32);
                return slowEquals(expectedSubkey, actualSubkey);
            }

            // ASP.NET Identity V3 format marker (0x01)
            if (formatMarker == 0x01) {
                if (decoded.length < 13) {
                    return false;
                }

                int prf = readNetworkByteOrder(decoded, 1);
                int iterCount = readNetworkByteOrder(decoded, 5);
                int saltLength = readNetworkByteOrder(decoded, 9);

                if (saltLength < 16 || 13 + saltLength > decoded.length) {
                    return false;
                }

                byte[] salt = new byte[saltLength];
                System.arraycopy(decoded, 13, salt, 0, saltLength);

                int subkeyLength = decoded.length - 13 - saltLength;
                if (subkeyLength < 16) {
                    return false;
                }

                byte[] expectedSubkey = new byte[subkeyLength];
                System.arraycopy(decoded, 13 + saltLength, expectedSubkey, 0, subkeyLength);

                String algorithm;
                switch (prf) {
                    case 0:
                        algorithm = "PBKDF2WithHmacSHA1";
                        break;
                    case 1:
                        algorithm = "PBKDF2WithHmacSHA256";
                        break;
                    case 2:
                        algorithm = "PBKDF2WithHmacSHA512";
                        break;
                    default:
                        return false;
                }

                byte[] actualSubkey = pbkdf2(plainPassword, salt, algorithm, iterCount, subkeyLength);
                return slowEquals(expectedSubkey, actualSubkey);
            }
        } catch (IllegalArgumentException e) {
            // Not base64 / not ASP.NET Identity format.
            return false;
        } catch (Exception e) {
            return false;
        }

        return false;
    }

    private static byte[] pbkdf2(String password, byte[] salt, String algorithm, int iterations, int outputBytes)
            throws Exception {
        PBEKeySpec spec = new PBEKeySpec(password.toCharArray(), salt, iterations, outputBytes * 8);
        SecretKeyFactory skf = SecretKeyFactory.getInstance(algorithm);
        return skf.generateSecret(spec).getEncoded();
    }

    private static int readNetworkByteOrder(byte[] buffer, int offset) {
        return (buffer[offset] & 0xFF) << 24
                | (buffer[offset + 1] & 0xFF) << 16
                | (buffer[offset + 2] & 0xFF) << 8
                | (buffer[offset + 3] & 0xFF);
    }

    private static boolean slowEquals(byte[] a, byte[] b) {
        if (a == null || b == null || a.length != b.length) {
            return false;
        }

        int diff = 0;
        for (int i = 0; i < a.length; i++) {
            diff |= a[i] ^ b[i];
        }
        return diff == 0;
    }

    @Override
    public Stream<UserModel> searchForUserStream(RealmModel realm, Map<String, String> params,
            Integer firstResult, Integer maxResults) {
  
        return fetchUsers(realm, params, firstResult != null ? firstResult : 0,
                maxResults != null ? maxResults : 100).stream();
    }

    private List<UserModel> fetchUsers(RealmModel realm, Map<String, String> params, int firstResult, int maxResults) {
        List<UserModel> users = new ArrayList<>();

        try (Connection conn = getConnection()) {
                String sql = "SELECT id, nome_login, nome_completo FROM usuarios " +
                    "WHERE dh_exclusao IS NULL " +
                    "AND (nome_login ILIKE ? OR nome_completo ILIKE ?) " +
                    "ORDER BY nome_completo LIMIT ? OFFSET ?";

            PreparedStatement stmt = conn.prepareStatement(sql);

            String search = params.get("keycloak.session.realm.users.query.search");
            if (search == null) {
                search = params.get("search");
            }
            String param = "%" + (search != null ? search : "") + "%";
            stmt.setString(1, param);
            stmt.setString(2, param);
            stmt.setInt(3, maxResults > 0 ? maxResults : 100);
            stmt.setInt(4, Math.max(firstResult, 0));

            ResultSet rs = stmt.executeQuery();
            while (rs.next()) {
                String id = rs.getString("id");
                String login = rs.getString("nome_login");
                String nomeCompleto = rs.getString("nome_completo");
                String firstName = (nomeCompleto == null || nomeCompleto.isBlank()) ? login : nomeCompleto.split(" ")[0];
                String email = login + "@feak.local";
                users.add(new FederatedUserAdapter(session, realm, model, id, login, email, firstName, true, null, null, null, null, null, null));
            }
        } catch (SQLException e) {
            log.error("Erro ao buscar usuários", e);
        }

        return users;
    }

    @Override
    public int getUsersCount(RealmModel realm) {

        String sql = "SELECT COUNT(*) FROM usuarios WHERE dh_exclusao IS NULL";

        try (Connection conn = getConnection()) {
            PreparedStatement st = conn.prepareStatement(sql);
            ResultSet rs = st.executeQuery();
            if (rs.next()) {
                return rs.getInt(1);
            }
        } catch (SQLException e) {
            log.error("Erro ao contar usuários", e);
        }
        return 0;
    }

    private Set<String> getRoles(Connection conn, String userId) throws SQLException {
        // Método não utilizado com novo schema
        return new HashSet<>();
    }

    public List<Map<String, Object>> getRolesAsMap(Connection conn, String userId) throws SQLException {
        // Método não utilizado com novo schema
        return new ArrayList<>();
    }

    public List<Map<String, Object>> getEmpresasSecundarias(Connection conn, String userId) throws SQLException {
        // Método não utilizado com novo schema
        return new ArrayList<>();
    }

    public Map<String, Object> getPerfisPemais(Connection conn, String IdsCdUsuario) throws SQLException {
        // Método não utilizado com novo schema
        return new HashMap<>();
    }

    @Override
    public Stream<UserModel> getGroupMembersStream(RealmModel realm, GroupModel group, Integer firstResult,
            Integer maxResults) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'getGroupMembersStream'");
    }

    @Override
    public Stream<UserModel> searchForUserByUserAttributeStream(RealmModel realm, String attrName, String attrValue) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'searchForUserByUserAttributeStream'");
    }

}

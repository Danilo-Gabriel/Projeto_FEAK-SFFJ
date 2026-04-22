package com.example.keycloak.federation;

import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;

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
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import org.jboss.logging.Logger;
import java.util.stream.Stream;

public class CustomUserStorageProvider
        implements UserStorageProvider, UserLookupProvider, CredentialInputValidator, UserQueryProvider {

    private final KeycloakSession session;
    private final ComponentModel model;
    private final HikariDataSource ds;

    private static final Logger log = Logger.getLogger(CustomUserStorageProvider.class);

    public CustomUserStorageProvider(KeycloakSession session, ComponentModel model) {
        this.session = session;
        this.model = model;

        String jdbcUrl = model.get(CustomUserStorageProviderFactory.DB_URL)
                + ";encrypt=true;trustServerCertificate=true";
        String user = model.get(CustomUserStorageProviderFactory.DB_USER);
        String password = model.get(CustomUserStorageProviderFactory.DB_PASSWORD);

        HikariConfig config = new HikariConfig();
        config.setJdbcUrl(jdbcUrl);
        config.setUsername(user);
        config.setPassword(password);

        this.ds = new HikariDataSource(config);
    }

    @Override
    public UserModel getUserByUsername(RealmModel realm, String cpf) {

        String sqlUser = "SELECT DISTINCT U.sCdUsuario as IdsCdUsuario, UC.sNrCPF as id, sNmUsuario as username, sDsEmail as email, E.nCdEmpresa as idEmpresa, CONCAT(E.sNmEmpresa, ' - ', E.sNmFantasia) as unidade, UV.sNmUsuarioVinculo AS vinculo " +
                         "FROM USUARIO U INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = U.sCdUsuario " +
                         "INNER JOIN USUARIO_VINCULO UV ON UV.nCdUsuarioVinculo = U.nCdUsuarioVinculo " +
                         "INNER JOIN EMPRESA E ON E.nCdEmpresa = u.nCdEmpresa " +
                         "WHERE UC.sNrCPF is not null and  UC.sNrCPF <> '' and UC.sNrCPF = ?";

        try (Connection conn = ds.getConnection();
                PreparedStatement pUser = conn
                        .prepareStatement(sqlUser)) {

            pUser.setString(1, cpf);
            ResultSet rUser = pUser.executeQuery();

            if (rUser.next()) {
                String id = rUser.getString("id");
                String name = rUser.getString("username");
                String email = rUser.getString("email");
                Map<String, Object> unidade = new HashMap<>();
                unidade.put("id", rUser.getString("idEmpresa"));
                unidade.put("unidade", rUser.getString("unidade"));
                String vinculo = rUser.getString("vinculo");
                String firstName = name.split(" ")[0];
                // Set<String> roles = getRoles(conn, id);
                List<Map<String, Object>> perfis = getRolesAsMap(conn, id);
                List<Map<String, Object>> empresas = getEmpresasSecundarias(conn, id);
                Map<String, Object> perfisPemais = getPerfisPemais(conn, rUser.getString("IdsCdUsuario"));

                return new FederatedUserAdapter(session, realm, model, id, name, email, firstName, true, null, perfis, unidade, vinculo, empresas, perfisPemais);

            }

        } catch (Exception e) {

            e.printStackTrace();
        }

        return null;
    }

    @Override
    public UserModel getUserByEmail(RealmModel realm, String email) {

        String sqlUser = "SELECT DISTINCT U.sCdUsuario as IdsCdUsuario, UC.sNrCPF as id, sNmUsuario as username, sDsEmail as email, E.nCdEmpresa as idEmpresa, CONCAT(E.sNmEmpresa, ' - ', E.sNmFantasia) as unidade, UV.sNmUsuarioVinculo AS vinculo " +
                         "FROM USUARIO U INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = U.sCdUsuario " +
                         "INNER JOIN USUARIO_VINCULO UV ON UV.nCdUsuarioVinculo = U.nCdUsuarioVinculo " +
                         "INNER JOIN EMPRESA E ON E.nCdEmpresa = u.nCdEmpresa " +
                         "WHERE UC.sNrCPF is not null and  UC.sNrCPF <> '' and U.sDsEmail = ?";

        try (Connection conn = ds.getConnection();
                PreparedStatement pUser = conn
                        .prepareStatement(sqlUser)) {

            pUser.setString(1, email);
            ResultSet rUser = pUser.executeQuery();

            if (rUser.next()) {
                String id = rUser.getString("id");
                String name = rUser.getString("username");
                Map<String, Object> unidade = new HashMap<>();
                unidade.put("id", rUser.getString("idEmpresa"));
                unidade.put("unidade", rUser.getString("unidade"));
                String vinculo = rUser.getString("vinculo");
                String firstName = name.split(" ")[0];
                // Set<String> roles = getRoles(conn, id);
                List<Map<String, Object>> perfis = getRolesAsMap(conn, id);
                List<Map<String, Object>> empresas = getEmpresasSecundarias(conn, id);
                Map<String, Object> perfisPemais = getPerfisPemais(conn, rUser.getString("IdsCdUsuario"));

                return new FederatedUserAdapter(session, realm, model, id, name, email, firstName, true, null, perfis, unidade, vinculo, empresas, perfisPemais);

            }

        } catch (Exception e) {

            e.printStackTrace();
        }

        return null;
    }

    @Override
    public void close() {
        if (ds != null) {
            ds.close();
        }
    }

    @Override
    public UserModel getUserById(RealmModel realm, String id) {

        StorageId storageId = new StorageId(id);
        String externalId = storageId.getExternalId();



          String sqlUser = "SELECT DISTINCT U.sCdUsuario as IdsCdUsuario, UC.sNrCPF as id, sNmUsuario as username, sDsEmail as email, E.nCdEmpresa as idEmpresa, CONCAT(E.sNmEmpresa, ' - ', E.sNmFantasia) as unidade, UV.sNmUsuarioVinculo AS vinculo " +
                         "FROM USUARIO U INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = U.sCdUsuario " +
                         "INNER JOIN USUARIO_VINCULO UV ON UV.nCdUsuarioVinculo = U.nCdUsuarioVinculo " +
                         "INNER JOIN EMPRESA E ON E.nCdEmpresa = u.nCdEmpresa " +
                         "WHERE UC.sNrCPF is not null and  UC.sNrCPF <> '' and UC.sNrCPF = ?";

        try (Connection conn = ds.getConnection();
                PreparedStatement pUser = conn.prepareStatement(sqlUser)) {

            pUser.setString(1, externalId);
            ResultSet rUser = pUser.executeQuery();

            if (rUser.next()) {
                String uname = rUser.getString("username");
                String email = rUser.getString("email");
                Map<String, Object> unidade = new HashMap<>();
                unidade.put("id", rUser.getString("idEmpresa"));
                unidade.put("unidade", rUser.getString("unidade"));
                String vinculo = rUser.getString("vinculo");
                String firstName = uname.split(" ")[0];
                // Set<String> roles = getRoles(conn, externalId);
                List<Map<String, Object>> perfis = getRolesAsMap(conn, externalId);
                List<Map<String, Object>> empresas = getEmpresasSecundarias(conn, externalId);
                Map<String, Object> perfisPemais = getPerfisPemais(conn, rUser.getString("IdsCdUsuario"));

                return new FederatedUserAdapter(session, realm, model, externalId, uname, email, firstName, true, null, perfis, unidade, vinculo, empresas, perfisPemais);
            }
        } catch (Exception e) {
            System.err.println("Erro em getUserById: " + e.getMessage());
            e.printStackTrace();
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
        StorageId storageId = new StorageId(user.getId());
        String id = storageId.getExternalId();

        try (Connection conn = ds.getConnection();
                PreparedStatement stmt = conn
                        .prepareStatement(
                                "SELECT sDsSenha FROM USUARIO U inner join USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = U.sCdUsuario  WHERE UC.sNrCPF = ?")) {
            stmt.setString(1, id);
            try (ResultSet rs = stmt.executeQuery()) {
                if (rs.next()) {

                    return checkPassword(cred.getValue(), rs.getString("sDsSenha"));
                }
            }
        } catch (Exception e) {
            System.err.println("Erro no isValid para usuário: " + user.getUsername());
            e.printStackTrace();
        }
        return false;
    }

    public static boolean checkPassword(String plainPassword, String storedHash) {
        String hashed = DigestUtils.md5Hex(plainPassword).toUpperCase();
        return hashed.equals(storedHash);
    }

    @Override
    public Stream<UserModel> searchForUserStream(RealmModel realm, Map<String, String> params,
            Integer firstResult, Integer maxResults) {
  
        return fetchUsers(realm, params, firstResult != null ? firstResult : 0,
                maxResults != null ? maxResults : 100).stream();
    }

    private List<UserModel> fetchUsers(RealmModel realm, Map<String, String> params, int firstResult, int maxResults) {
        List<UserModel> users = new ArrayList<>();

        try (Connection conn = ds.getConnection()) {
            String sql = "SELECT DISTINCT UC.sNrCPF as id, U.sNmUsuario as username, U.sDsEmail as email " +
                    "FROM USUARIO U " +
                    "INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = U.sCdUsuario " +
                    "WHERE UC.sNrCPF IS NOT NULL AND UC.sNrCPF <> '' " +
                    "AND (LOWER(U.sNmUsuario) LIKE LOWER(?) " +
                    "     OR LOWER(U.sDsEmail) LIKE LOWER(?) " +
                    "     OR UC.sNrCPF LIKE ?) " +
                    "ORDER BY U.sNmUsuario OFFSET ? ROWS FETCH NEXT ? ROWS ONLY";

            PreparedStatement stmt = conn.prepareStatement(sql);

            String search = params.get("keycloak.session.realm.users.query.search").toString();
            String param = "%" + search + "%";
            stmt.setString(1, param);
            stmt.setString(2, param);
            stmt.setString(3, param);
            stmt.setInt(4, firstResult); // OFFSET
            stmt.setInt(5, maxResults > 0 ? maxResults : 100);

            ResultSet rs = stmt.executeQuery();
            while (rs.next()) {
                String id = rs.getString("id");
                String username = rs.getString("username");
                String email = rs.getString("email");
                String firstName = username.split(" ")[0];
                users.add( new FederatedUserAdapter(session, realm, model, id, username, email, firstName, true, null, null, null, null, null, null));
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }

        return users;
    }

    @Override
    public int getUsersCount(RealmModel realm) {

        String sql = "SELECT COUNT(*) FROM USUARIO U " +
                      "INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = U.sCdUsuario " +
                      "WHERE UC.sNrCPF IS NOT NULL AND UC.sNrCPF <> '' ";

        try (Connection conn = ds.getConnection()) {
            PreparedStatement st = conn.prepareStatement(sql);
            ResultSet rs = st.executeQuery();
            if (rs.next()) {
                return rs.getInt(1);
            }
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
        return 0;
    }

    private Set<String> getRoles(Connection conn, String userId) throws SQLException {
        Set<String> roles = new HashSet<>();
        String sqlRole = "SELECT DISTINCT GS.sDsGrupo as nome " +
                "FROM GRUPO_USUARIO as GU " +
                "INNER JOIN GRUPO_SISTEMA AS GS ON GU.nCdGrupo = GS.nCdGrupo " +
                "INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = GU.sCdUsuario " +
                "WHERE GS.bFlVisivel = 1 AND UC.sNrCPF = ?";

        try (PreparedStatement stmt = conn.prepareStatement(sqlRole)) {
            stmt.setString(1, userId);
            ResultSet rs = stmt.executeQuery();
            while (rs.next()) {
                roles.add(rs.getString("nome"));
            }
        }
        return roles;
    }

    public List<Map<String, Object>> getRolesAsMap(Connection conn, String userId) throws SQLException {

        List<Map<String, Object>> perfis = new ArrayList<>();

        String sqlRole = "SELECT GS.nCdGrupo as id, GS.sDsGrupo as nome, T.sDsTipo as tipo " +
                "FROM GRUPO_USUARIO GU " +
                "INNER JOIN GRUPO_SISTEMA GS ON GU.nCdGrupo = GS.nCdGrupo " +
                "INNER JOIN TIPO T ON GS.nCdTipo  = T.nCdTipo " +
                "INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = GU.sCdUsuario " +
                "WHERE GS.bFlVisivel = 1 AND UC.sNrCPF = ?";
        try (PreparedStatement stmt = conn.prepareStatement(sqlRole)) {
            stmt.setString(1, userId);
            ResultSet rs = stmt.executeQuery();
            while (rs.next()) {
                Map<String, Object> map = new HashMap<>();
                map.put("id", rs.getString("id"));
                map.put("nome", rs.getString("nome"));
                map.put("tipo", rs.getString("tipo"));
                perfis.add(map);

            }
        }
        return perfis;
    }

        public List<Map<String, Object>> getEmpresasSecundarias(Connection conn, String userId) throws SQLException {
        List<Map<String, Object>> empresas = new ArrayList<>();

        String sql = "SELECT DISTINCT UC.sNrCPF, E.nCdEmpresa as id, E.sNmEmpresa as nome " +
                "FROM USUARIO  U " +
                "INNER JOIN USUARIO_EMPRESA EM ON EM.sCdUsuario = U.sCdUsuario " +
                "INNER JOIN EMPRESA_TIPO ET ON ET.nCdEmpresa = EM.nCdEmpresa " +
                "INNER JOIN USUARIO_COMPLEMENTO UC ON UC.sCdUsuario = U.sCdUsuario " +
                "INNER JOIN EMPRESA E ON E.nCdEmpresa = EM.nCdEmpresa " +
                "WHERE U.sCdUsuario is not null and  U.sCdUsuario <> '' and UC.sNrCPF = ?";

        try (PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setString(1, userId);
            ResultSet rs = stmt.executeQuery();
            while (rs.next()) {
                Map<String, Object> map = new HashMap<>();
                map.put("id", rs.getString("id"));
                map.put("nome", rs.getString("nome"));
                empresas.add(map);
            }
        }
        return empresas;
    }

    public Map<String, Object> getPerfisPemais(Connection conn, String IdsCdUsuario) throws SQLException {

        Map<String, Object> perfisPemais =  new HashMap<>();

        String sql = "SELECT pp.Id as id, pp.Nome as nome FROM PEMAIS_PESSOA_PERFIS PPP " +
                     "INNER JOIN PEMAIS_PERFIS pp ON PP.Id = PPP.IdPemaisPerfis " +
                     "WHERE pp.Status = 1 and PPP.IdsCdUsuario = ?";

        try (PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setString(1, IdsCdUsuario);
            ResultSet rs = stmt.executeQuery();
            while (rs.next()) {
                perfisPemais.put("id", rs.getString("id"));
                perfisPemais.put("nome", rs.getString("nome"));
            }
        }

        log.info("PerfisPemais" + perfisPemais);
        return perfisPemais;
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

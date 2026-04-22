package com.example.keycloak.mappers;

import org.keycloak.models.KeycloakSession;
import org.keycloak.models.UserSessionModel;
import org.keycloak.models.ClientSessionContext;
import org.keycloak.protocol.oidc.mappers.AbstractOIDCProtocolMapper;
import org.keycloak.protocol.oidc.mappers.OIDCAccessTokenMapper;
import org.keycloak.representations.AccessToken;
import org.keycloak.representations.IDToken;
import org.keycloak.models.ProtocolMapperModel;
import org.keycloak.provider.ProviderConfigProperty;
import org.jboss.logging.Logger;

import com.example.keycloak.federation.FederatedUserAdapter;
import org.keycloak.models.UserModel;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class CustomProtocolMapper extends AbstractOIDCProtocolMapper implements OIDCAccessTokenMapper {

    public static final String PROVIDER_ID = "roles-custom-mapper";
    private static final String CLAIM_PERFIL = "perfis";
    private static final String CLAIM_PERFIL_PEMAIS = "perfilPemais";
    private static final String CLAIN_UNIDADE = "unidade";
    private static final String CLAIM_EMPRESAS = "empresas";
    private static final String CLAIM_USUARIO_TIPO = "usuario_tipo";

    private static final Logger log = Logger.getLogger(CustomProtocolMapper.class);

    private static final List<ProviderConfigProperty> configProperties = new ArrayList<>();

    public CustomProtocolMapper() {

    }

    static {
        // ProviderConfigProperty claimNameProp = new ProviderConfigProperty();
        // claimNameProp.setName("claim.name");
        // claimNameProp.setLabel("Claim Name");
        // claimNameProp.setType(ProviderConfigProperty.STRING_TYPE);
        // claimNameProp.setHelpText("Nome do claim que será adicionado ao token");
        // configProperties.add(claimNameProp);
    }

    @Override
    protected void setClaim(IDToken token, ProtocolMapperModel mappingModel,
            UserSessionModel userSession, KeycloakSession keycloakSession,
            ClientSessionContext clientSessionCtx) {
        UserModel user = userSession.getUser();

        Map<String, Object> unidade = new HashMap<>();
        Map<String, Object> perfilPemais = new HashMap<>();
        String vinculo = "";
        List<Map<String, Object>> perfis = new ArrayList<>();
        List<Map<String, Object>> empresas = new ArrayList<>();

        if (user instanceof FederatedUserAdapter) {
            FederatedUserAdapter federatedUser = (FederatedUserAdapter) user;
            try {
                perfis = federatedUser.getPerfis();
                empresas = federatedUser.getEmpresaSecundarias();
                unidade = federatedUser.getUnidade();
                vinculo = federatedUser.getVinculo();
                perfilPemais = federatedUser.getPefilPemais();

            } catch (Exception e) {
                log.error("Erro ao buscar perfis do usuário: " + e.getMessage(), e);
            }
        }

        token.getOtherClaims().put(CLAIM_PERFIL, perfis);
        token.getOtherClaims().put(CLAIM_PERFIL_PEMAIS, perfilPemais);
        token.getOtherClaims().put(CLAIN_UNIDADE, unidade);
        token.getOtherClaims().put(CLAIM_EMPRESAS, empresas);
        token.getOtherClaims().put(CLAIM_USUARIO_TIPO, vinculo);

    }

    @Override
    public AccessToken transformAccessToken(AccessToken token, ProtocolMapperModel mappingModel,
            KeycloakSession session, UserSessionModel userSession,
            ClientSessionContext clientSessionCtx) {

        UserModel user = userSession.getUser();
        Map<String, Object> unidade = new HashMap<>();
        String vinculo = "";
        List<Map<String, Object>> perfis = new ArrayList<>();
        List<Map<String, Object>> empresas = new ArrayList<>();
        Map<String, Object> perfilPemais = new HashMap<>();

        if (user instanceof FederatedUserAdapter) {
            FederatedUserAdapter federatedUser = (FederatedUserAdapter) user;
            try {
                perfis = federatedUser.getPerfis();
                empresas = federatedUser.getEmpresaSecundarias();
                unidade = federatedUser.getUnidade();
                vinculo = federatedUser.getVinculo();
                perfilPemais = federatedUser.getPefilPemais();

            } catch (Exception e) {
                log.error("Erro ao buscar perfis do usuário: " + e.getMessage(), e);
            }
        }

        token.getOtherClaims().put(CLAIM_PERFIL, perfis);
        token.getOtherClaims().put(CLAIM_PERFIL_PEMAIS, perfilPemais);
        token.getOtherClaims().put(CLAIN_UNIDADE, unidade);
        token.getOtherClaims().put(CLAIM_EMPRESAS, empresas);
        token.getOtherClaims().put(CLAIM_USUARIO_TIPO, vinculo);

        return token;

    }

    @Override
    public List<ProviderConfigProperty> getConfigProperties() {
        return configProperties;
    }

    @Override
    public String getDisplayCategory() {
        return TOKEN_MAPPER_CATEGORY;
    }

    @Override
    public String getDisplayType() {
        return "Custom mapper pe-integrado";
    }

    @Override
    public String getHelpText() {
        return "Mapper PE_INTEGRADO";
    }

    @Override
    public String getId() {
        return PROVIDER_ID;
    }
}

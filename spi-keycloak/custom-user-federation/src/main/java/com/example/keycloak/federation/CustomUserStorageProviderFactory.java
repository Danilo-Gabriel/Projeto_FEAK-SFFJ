package com.example.keycloak.federation;

import org.keycloak.component.ComponentModel;
import org.keycloak.models.KeycloakSession;
import org.keycloak.provider.ProviderConfigProperty;
import org.keycloak.storage.UserStorageProviderFactory;

import java.util.ArrayList;
import java.util.List;

public class CustomUserStorageProviderFactory implements UserStorageProviderFactory<CustomUserStorageProvider> {

    public static final String PROVIDER_ID = "FEAK-JDBC";

    public static final String DB_URL = "dbUrl";
    public static final String DB_USER = "dbUser";
    public static final String DB_PASSWORD = "dbPassword";

    @Override
    public CustomUserStorageProvider create(KeycloakSession session, ComponentModel model) {
        return new CustomUserStorageProvider(session, model);
    }

    @Override
    public String getId() {
        return PROVIDER_ID;
    }

    @Override
    public String getHelpText() {
        return "Base de dados alternativa da FEAK para autenticação de usuários";
    }

    @Override
    public List<ProviderConfigProperty> getConfigProperties() {
        List<ProviderConfigProperty> config = new ArrayList<>();

        ProviderConfigProperty url = new ProviderConfigProperty();
        url.setName(DB_URL);
        url.setLabel("Database URL");
        url.setType(ProviderConfigProperty.STRING_TYPE);
        url.setHelpText("JDBC URL do banco de dados");
        config.add(url);

        ProviderConfigProperty user = new ProviderConfigProperty();
        user.setName(DB_USER);
        user.setLabel("Database User");
        user.setType(ProviderConfigProperty.STRING_TYPE);
        user.setHelpText("Usuário do banco de dados");
        config.add(user);

        ProviderConfigProperty password = new ProviderConfigProperty();
        password.setName(DB_PASSWORD);
        password.setLabel("Database Password");
        password.setType(ProviderConfigProperty.PASSWORD);
        password.setHelpText("Senha do banco de dados");
        config.add(password);

        return config;
    }
}

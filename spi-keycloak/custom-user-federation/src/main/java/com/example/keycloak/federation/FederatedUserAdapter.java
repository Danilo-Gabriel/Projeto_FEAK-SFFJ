package com.example.keycloak.federation;

import org.keycloak.models.RealmModel;
import org.keycloak.models.RoleModel;
import org.keycloak.storage.StorageId;
import org.keycloak.storage.adapter.AbstractUserAdapterFederatedStorage;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.keycloak.component.ComponentModel;
import org.keycloak.models.KeycloakSession;

public class FederatedUserAdapter extends AbstractUserAdapterFederatedStorage {

    private String externalId;
    private String email;
    private String username;
    private String firstName;
    private Map<String, Object> unidade;
    private Map<String, Object> perfisPemais;
    private String vinculo;
    private Set<String> roles;
    private List<Map<String, Object>> perfis;
    private List<Map<String, Object>> empresas;

    public FederatedUserAdapter(KeycloakSession session, RealmModel realm, ComponentModel model, String externalId,
            String username, String email, String firstName, boolean enabled, Set<String> roles,
            List<Map<String, Object>> perfis, Map<String, Object> unidade, String vinculo, List<Map<String, Object>> empresas, Map<String, Object> perfisPemais) {
        super(session, realm, model);
        this.firstName = firstName;
        this.externalId = externalId;
        this.username = username;
        this.email = email;
        this.unidade = unidade;
        this.vinculo = vinculo;
        this.roles = roles != null ? roles : new HashSet<>();
        this.perfis = perfis != null ? perfis : new ArrayList<Map<String, Object>>();
        this.empresas = empresas != null ? empresas : new ArrayList<Map<String, Object>>();
        this.perfisPemais = perfisPemais;
        setUsername(username);
        setEmail(email);
        setFirstName(firstName);
        setEnabled(enabled);

    }

    @Override
    protected Set<RoleModel> getRoleMappingsInternal() {
        Set<RoleModel> roleModels = new HashSet<>();
        for (String roleName : roles) {
            RoleModel role = realm.getRole(roleName);
            if (role == null) {
                role = realm.addRole(roleName);
            }
            roleModels.add(role);
        }
        return roleModels;
    }

    public Map<String, Object> getUnidade(){
        return unidade;
    }

    public Map<String, Object> getPefilPemais(){
        return perfisPemais;
    }

    public String getVinculo(){
        return vinculo;
    }

    public Set<String> getRoles() {
        return roles;
    }

    public List<Map<String, Object>> getPerfis() {
        return perfis;
    }

    public List<Map<String, Object>> getEmpresaSecundarias() {
        return empresas;
    }

    @Override
    public boolean hasRole(RoleModel role) {
        return roles.contains(role.getName());
    }

    @Override
    public String getId() {
        return StorageId.keycloakId(storageProviderModel, externalId);
    }

    @Override
    public String getEmail() {
        return email;
    }

    @Override
    public boolean isEnabled() {
        return super.isEnabled();
    }

    @Override
    public void setEmail(String email) {
        this.email = email;
        super.setEmail(email);

    }

    @Override
    public String getUsername() {
        return username;
    }

    @Override
    public void setUsername(String username) {
        this.username = username;
        super.setFirstName(username);

    }

}

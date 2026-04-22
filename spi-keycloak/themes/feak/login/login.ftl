<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>FEAK | Sistema Financeiro</title>
    <link href="${url.resourcesPath}/css/styles.css" rel="stylesheet" />
</head>
<body>
    <main class="login-shell">
        <section class="login-brand">
            <img src="${url.resourcesPath}/images/doctors-animate.svg" alt="Ilustração FEAK">
            <h1>FEAK - SFFJ</h1>
            <p>Ambiente seguro para operação, gestão e acompanhamento financeiro.</p>
        </section>

        <section class="login-card">
            <span class="eyebrow">Acesso ao sistema</span>
            <h2>Entrar</h2>

            <form id="kc-form-login" action="${url.loginAction}" method="post">
                <label for="username">Usuário</label>
                <input
                    type="text"
                    id="username"
                    name="username"
                    placeholder="Informe seu usuário"
                    autofocus
                    autocomplete="username"
                    required
                />

                <label for="password">Senha</label>
                <input
                    type="password"
                    id="password"
                    name="password"
                    placeholder="Informe sua senha"
                    autocomplete="current-password"
                    required
                />

                <div class="form-options">
                    <label class="checkbox">
                        <input type="checkbox" id="showPassword" />
                        Mostrar senha
                    </label>

                    <#if realm.rememberMe>
                        <label class="checkbox">
                            <input type="checkbox" id="rememberMe" name="rememberMe" />
                            Lembrar-me
                        </label>
                    </#if>
                </div>

                <button type="submit" name="login">Entrar</button>
            </form>

            <#if message?has_content>
                <div class="alert ${message.type}">
                    ${message.summary}
                </div>
            </#if>
        </section>
    </main>

    <script>
        document.getElementById('showPassword').addEventListener('change', function(event) {
            const password = document.getElementById('password');
            password.type = event.target.checked ? 'text' : 'password';
        });
    </script>
</body>
</html>
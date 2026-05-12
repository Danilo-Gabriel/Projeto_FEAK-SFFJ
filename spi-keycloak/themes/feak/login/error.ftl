<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Erro de autenticacao</title>
    <link href="${url.resourcesPath}/css/styles.css" rel="stylesheet" />
</head>
<body>
    <main class="login-shell" style="grid-template-columns: 1fr; max-width: 760px;">
        <section class="login-card" style="text-align: center;">
            <span class="eyebrow">Autenticacao</span>
            <h2>Erro no login</h2>
            <div class="alert error" style="display: block; margin-top: 8px;">
                <#if message?has_content>
                    ${kcSanitize(message.summary)?no_esc}
                <#else>
                    Ocorreu um erro durante a autenticacao.
                </#if>
            </div>
            <p style="margin-top: 16px; color: #42657b;">
                Tente novamente em instantes.
            </p>
            <p style="margin-top: 18px;">
                <a href="${url.loginUrl}" style="display:inline-block;padding:10px 20px;border-radius:10px;background:#0f5d88;color:#fff;text-decoration:none;font-weight:700;">
                    Voltar ao login
                </a>
            </p>
        </section>
    </main>
</body>
</html>

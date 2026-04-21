<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login</title>
    <link href="${url.resourcesPath}/css/styles.css" rel="stylesheet" />
</head>
<body>
    <div class="login-container">
        <img src="${url.resourcesPath}/images/logo.png" alt="Logo">
        <h1>Bem-vindo ao Sistema</h1>
        <!-- FORMULÁRIO TRADICIONAL -->
        <form id="kc-form-login" action="${url.loginAction}" method="post">
            <input 
                type="text" 
                id="username" 
                name="username" 
                placeholder="Usuário"  
                autofocus 
                autocomplete="off" 
                maxlength="11" 
                pattern="\d{11}" 
                title="Digite apenas números do CPF (11 dígitos)" 
                required
                />

            <input type="password" id="password" name="password" placeholder="Senha"/>
            
            <div class="form-options">

                <#if true> 
                
                <div>
                     <label class="checkbox">
                     <input type="checkbox" onclick="myFunction()"/>
                    Mostrar senha
                </label>
         
             <!--  Alterar para true se precisar de "Lembrar-me" -->
                <label class="checkbox">
                    <input type="checkbox" id="rememberMe" name="rememberMe" />
                    Lembrar-me 
                </label>

                </div>
              
                <label for="">
                    <a href="#">Esqueceu a senha?</a>
                </label>
                </#if>
            </div>
            <input type="submit" name="login" value="Entrar"/>
        </form>
            <div class="separator">
                <span>ou</span>
            </div>

               <!-- BOTÃO expresso pe -->
         <#--  <#if social.providers??> 
            <div class="social-login">
                 <#list social.providers as provider>
                    <a href="${provider.loginUrl}" class="btn-social btn-${provider.alias}">
                         ${provider.displayName!} 
                        Entrar com Login Único GovBR
                    </a>
                 </#list> 
            </div>
            
            
         </#if>   -->
         <#if message?has_content> 
    <div class="alert ${message.type}">
         ${message.summary} 
    </div>
         </#if> 
        <script>
        // Adicionando efeitos interativos
        document.addEventListener('DOMContentLoaded', function() {
            const inputs = document.querySelectorAll('input');
            inputs.forEach(input => {
                input.addEventListener('focus', function() {
                    this.parentNode.classList.add('focused');
                });
                input.addEventListener('blur', function() {
                    if (this.value === '') {
                        this.parentNode.classList.remove('focused');
                    }
                });
            });
        });

 
    document.getElementById("username").addEventListener("input", function(e) {
        let value = e.target.value.replace(/\D/g, ""); // só números

        e.target.value = value;
    });

    function myFunction() {
  var x = document.getElementById("password");
  if (x.type === "password") {
    x.type = "text";
  } else {
    x.type = "password";
  }
}
    </script>
</body>
</html>
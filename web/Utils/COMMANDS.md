# Comandos úteis — Frontend

Execute os comandos a partir da raiz do repositório, salvo quando indicado.

## Acessar o frontend

```powershell
Set-Location web
```

## Instalar dependências

```powershell
npm install
```

Para instalação reproduzível usando exatamente o `package-lock.json`:

```powershell
npm ci
```

## Executar em desenvolvimento

```powershell
npm start
```

Aplicação: `http://localhost:4200`

## Gerar build de produção

```powershell
npm run build -- --configuration production
```

## Executar testes

Modo interativo:

```powershell
npm test
```

Execução única no Chrome Headless:

```powershell
npx ng test --watch=false --browsers=ChromeHeadless
```

Executar somente o teste do PDV:

```powershell
npx ng test --watch=false --browsers=ChromeHeadless --include=src/app/pages/pdv/pdv.component.spec.ts
```

## Limpar artefatos e reinstalar

```powershell
Remove-Item -Recurse -Force node_modules, build -ErrorAction SilentlyContinue
npm ci
```

## Verificar versões

```powershell
node --version
npm --version
npx ng version
```


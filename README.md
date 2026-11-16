# Sweet Home — Portal de imóveis

Portal de imóveis para **comprar, alugar e anunciar**, com busca e filtros, galeria de fotos,
contato com o anunciante, cadastro/login de usuários e um precificador de imóveis.
Projeto de portfólio, completo de ponta a ponta e pronto para rodar localmente.

## Tecnologias

| Camada | Stack |
|---|---|
| Front-end | Vue 3 (Composition API), Vue Router, Vite |
| Back-end | .NET 10 (ASP.NET Core Web API), Dapper |
| Banco de dados | SQLite (arquivo local, criado automaticamente) |
| Imagens | Pasta local por padrão; Cloudinary opcional |
| Segurança | Senhas com hash BCrypt, CORS restrito ao front |

## Funcionalidades

- Listagem de imóveis com busca por texto, tipo, finalidade (comprar/alugar), preço máximo e ordenação
- Página de detalhes com galeria, características, diferenciais e formulário de contato
- Cadastro e login de usuários (validação de CPF/CNPJ, preenchimento de endereço pelo CEP)
- Anúncio de imóveis com upload de várias fotos
- Precificador com estimativa de valor
- Minha conta: trocar foto, usuário e senha; excluir conta
- Área do administrador: mensagens recebidas e remoção de anúncios
- Newsletter e formulário de contato (mensagens ficam salvas no banco)

## Como rodar

**Requisitos:** [.NET SDK 10](https://dotnet.microsoft.com/download) e [Node.js 20+](https://nodejs.org/).

Opção rápida (API + front juntos): no Mac dê dois cliques em `Iniciar.command`; no Windows, em `Iniciar.bat`; ou, no terminal:

```bash
bash scripts/iniciar.sh
```

Ou manualmente, em dois terminais:

```bash
# 1) API  ->  http://localhost:5070  (documentação Swagger em /swagger)
dotnet run --project API

# 2) Front  ->  http://localhost:5173
cd FrontEnd/imobiliaria
cp .env.example .env      # só na primeira vez
npm install
npm run dev
```

Na primeira execução a API cria o banco (`API/App_Data/imobiliaria.db`), um usuário administrador
e 30 imóveis de exemplo.

**Administrador padrão (somente para uso local):** usuário `Admin`, senha `Admin`.
Para trocar a senha inicial, defina `Admin:Senha` em `API/appsettings.json` antes da primeira execução.

## Configuração

| O quê | Onde | Padrão |
|---|---|---|
| URL da API usada pelo front | `FrontEnd/imobiliaria/.env` → `VITE_API_URL` | `http://localhost:5070` |
| Origens permitidas (CORS) | `API/appsettings.json` → `Cors:Origens` | `http://localhost:5173` |
| Banco de dados | `ConnectionStrings:DefaultConnection` | `API/App_Data/imobiliaria.db` |
| Dados de exemplo | `Seed:ImoveisExemplo` | `true` |
| Cloudinary (opcional) | `Cloudinary:CloudName`, `ApiKey`, `ApiSecret` | vazio → uploads locais |

Para usar Cloudinary sem colocar segredos no repositório, use variáveis de ambiente
(`Cloudinary__CloudName`, `Cloudinary__ApiKey`, `Cloudinary__ApiSecret`) ou `dotnet user-secrets`.
Sem essas credenciais, as imagens enviadas ficam em `API/App_Data/uploads` e são servidas em `/uploads`.

## Estrutura

```
API/        Controllers, configuração e inicialização (Program.cs)
Core/       Serviços, repositórios (SQLite/Dapper) e armazenamento de imagens
Entidades/  Modelos, DTOs e interfaces
FrontEnd/imobiliaria/
  src/styles/       Tokens do design system e estilos base
  src/components/   ui/ (componentes base), layout/, imovel/
  src/views/        Telas
  src/services/     Cliente da API
docs/       Paleta de cores
scripts/    iniciar.sh e verificar-api.sh
```

## Design system

Cores, tipografia, espaçamento, raios e sombras ficam em `src/styles/tokens.css`.
Os componentes base (`BaseButton`, `BaseField`, `BaseAlert`, `BaseBadge`, `BaseCounter`,
`BaseSegmented`, `EmptyState`, `LoadingState`, `PageHeader`) estão em `src/components/ui/`
e são usados por todas as telas. Paleta original em [`docs/paleta-de-cores.md`](docs/paleta-de-cores.md).

## Verificação

```bash
bash scripts/verificar-api.sh
```

Compila a solução, sobe a API com um banco temporário, exercita os endpoints e faz o build do front.

## Observações

Este projeto é uma demonstração: o login não usa tokens de sessão e a área do administrador
é controlada apenas no front-end. Antes de publicar em produção, adicione autenticação (JWT ou cookies)
e autorização no servidor. As fotos dos imóveis de exemplo estão hospedadas em um serviço externo
e podem deixar de carregar se forem removidas de lá.

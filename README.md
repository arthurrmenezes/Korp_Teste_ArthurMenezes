Desafio Técnico Korp - Sistema de Emissão de Notas Fiscais

Este repositório contém a solução para o desafio técnico da empresa Korp, desenvolvido para a vaga de Estágio de Desenvolvimento Full Stack.

O projeto consiste em um sistema completo para gerenciamento de produtos e emissão de notas fiscais, implementado com uma arquitetura de microsserviços em .NET 8 (C#) e um frontend em Angular.

🚀 Funcionalidades Implementadas
O sistema atende a todos os requisitos funcionais e arquiteturais solicitados no desafio:

✔ Cadastro de Produtos: Gerenciamento de produtos com código, descrição e saldo.
✔ Cadastro de Notas Fiscais: Criação de notas com status inicial "Aberto" e adição de múltiplos produtos.
✔ Impressão de Notas Fiscais: Ação que altera o status para "Fechado" e realiza a dedução de estoque.
✔ Arquitetura de Microsserviços: Solução dividida em Stock Service (Estoque) e Billing Service (Faturamento).
✔ Controle de Concorrência: Implementação de lock pessimista para garantir que o estoque não fique negativo em operações simultâneas.
✔ Integração com IA: Um chatbot de suporte (integrado com API do Google Gemini) para responder perguntas sobre o sistema.

🏛️ Arquitetura e Detalhes Técnicos
Backend (.NET 8 / C#)
O backend foi estruturado em dois microsserviços independentes, cada um com seu próprio banco de dados PostgreSQL, seguindo os princípios da Clean Architecture e do Repository Pattern.
Frameworks: A solução utiliza .NET 8 e ASP.NET Core para a construção das APIs RESTful. O acesso a dados é feito com Entity Framework Core 8.
Tratamento de Exceções: Foi implementado um Middleware global de exceções em cada serviço.
Tratamento de Falhas (Polly): Para garantir a resiliência, o BillingService utiliza a biblioteca Polly para aplicar os padrões de Retry (Tentativa) e Circuit Breaker ao se comunicar com o StockService.
Controle de Concorrência (Pessimistic Lock): Para resolver o cenário de concorrência, foi utilizada uma transação com lock pessimista (SELECT FOR UPDATE) no banco de dados.
Uso de LINQ: O LINQ foi usado extensivamente com o Entity Framework para todas as consultas de dados, incluindo .Include() para carregar itens da nota, .FirstOrDefaultAsync() para buscas e .Skip().Take() para implementar a paginação.
IA (Gemini): A integração com IA foi feita criando um endpoint no BillingService.

Frontend (Angular)
O frontend é uma aplicação de página única (SPA) moderna e reativa.
Bibliotecas Visuais: A interface foi construída inteiramente com a biblioteca Angular Material, utilizando MatTable (tabelas), MatPaginator (paginação), MatSidenav (menu lateral) e MatDialog (pop-ups).
Bibliotecas de Dados: A comunicação com o backend é gerenciada pelo HttpClient do Angular.
Uso de RxJS: O RxJS é a base de toda a comunicação assíncrona. Cada chamada HttpClient retorna um Observable, que é consumido nos componentes usando .subscribe() para tratar as respostas de sucesso (next) e de erro (error), onde as mensagens do backend são traduzidas para o usuário.
Ciclos de Vida: O principal ciclo de vida utilizado foi o ngOnInit, usado para carregar os dados iniciais das tabelas (como loadProducts() e loadInvoices()) assim que os componentes são renderizados.
Linguagem: O projeto é 100% escrito em TypeScript.

🛠️ Como Executar o Projeto
Siga os passos abaixo para configurar e rodar a aplicação localmente.

Pré-requisitos
Você precisará ter as seguintes ferramentas instaladas:

.NET 8 SDK
Node.js e npm (que inclui o Angular CLI)
PostgreSQL Server (ou um container Docker com PostgreSQL)

1. Configuração do Backend (Serviços e Banco)
O backend possui dois projetos (BillingService e StockService) que precisam ser configurados e executados.

1.1. Banco de Dados (PostgreSQL)
Crie dois bancos de dados separados no seu PostgreSQL (stockdb e billingdb).

1.2. Arquivos de Configuração (appsettings.json)
Navegue até o projeto BillingService e abra o arquivo appsettings.json.
Adicione sua Connection String do PostgreSQL:
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=billingdb;Username=postgres;Password=SEU_PASSWORD"
}

Neste mesmo arquivo, adicione sua API Key do Google AI Studio (necessária para o chatbot):
"GEMINI_API_KEY": "SUA_API_KEY_DO_GEMINI_AQUI"

Navegue até o projeto StockService, abra o appsettings.json e adicione a Connection String para o banco de estoque:
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=stockdb;Username=postgres;Password=SEU_PASSWORD"
}

1.3. Migrations (Entity Framework)
Você precisa aplicar as migrations para que o EF Core crie as tabelas em ambos os bancos.
Execute os comandos abaixo no terminal para cada projeto de backend (BillingService e StockService):

# 1. Apague a pasta 'Migrations/' (camada Infrastructure)
# 2. Navegue para a pasta do projeto (ex: /BillingService)
# 3. Crie a migration inicial
dotnet ef migrations add Initial

# 4. Aplique a migration ao banco de dados
dotnet ef database update

1.4. Executar o Backend
Abra dois terminais separados:
No terminal 1, navegue até StockService/ e execute:
dotnet run
(Deve iniciar em https://localhost:7146)
No terminal 2, navegue até BillingService/ e execute:
dotnet run
(Deve iniciar em https://localhost:7154)

2. Configuração do Frontend (Angular)
No terminal, dentro da pasta frontend/KorpFrontend/:

# 1. Instale as dependências
npm install

# 2. Inicie a aplicação
npm start

O site estará disponível em http://localhost:4200/.


📖 Visão Geral da API (Endpoints)
📦 STOCK SERVICE: 
GET /api/product?pageNumber=&pageSize= 
Lista paginada de produtos. 

POST /api/product 
Cadastra um produto. 

PUT /api/product/balance/increment 
Incrementa o saldo de um produto. 

PUT /api/product/balance/decrement 
Decrementa o saldo de um produto. 

GET /api/product/{code} 
Busca produto por código. 

POST /api/product/lock 
Consulta produto com lock pessimista (SELECT FOR UPDATE). 

🧾 BILLING SERVICE 
POST /api/v1/invoices 
Cria nota fiscal. 

GET /api/v1/invoices 
Lista todas as notas fiscais com paginação.

GET /api/v1/invoices/{id} 
Busca uma nota fiscal pelo ID.

POST /api/invoices/{id}/add-product 
Adiciona produtos a uma nota fiscal que está com status Aberto. 

POST /api/invoices/{id}/print 
Imprime a nota fiscal, atualiza o estoque via Stock Service e altera o Status para Fechado. 

🤖 AI SERVICE 
POST /api/v1/ai/chat 
Chama a API do Google Gemini IA, recebendo como entrada a pergunta e retornando a resposta gerada pela IA.

# Desafio Fast Workshop - Back End

Repositório contendo o **back-end** do projeto **Desafio Fast Workshop**, desenvolvido com **.NET 6** e **MySQL**.  
Este projeto fornece a API consumida pelo front-end Angular disponível [aqui](https://github.com/luccahcs/desafio-fast-workshop-front-end).

---

## 📝 Descrição

O back-end implementa a lógica de negócios do Desafio Fast Workshop, fornecendo endpoints REST para gerenciamento de workshops.  
Inclui autenticação, rotas de CRUD e integração com banco de dados MySQL.

---

## ⚙️ Tecnologias

- **.NET 6**: Framework principal
- **C#**: Linguagem de desenvolvimento
- **MySQL**: Banco de dados relacional
- **Entity Framework Core**: ORM
- **Git/GitHub**: Controle de versão

---

## 🛠️ Pré-requisitos

Para rodar o back-end, é necessário:

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [MySQL](https://dev.mysql.com/downloads/mysql/) ou [Docker](https://www.docker.com/) para rodar o banco
- Um cliente MySQL opcional (como MySQL Workbench)

---

## 📥 Instalação

1. Clone o repositório:

git clone https://github.com/luccahcs/fast-workshops-back-end.git
Acesse a pasta da API:

cd fast-workshops-back-end/FastWorkshops.Api
Instale as dependências do .NET (opcionalmente restaurando pacotes NuGet):

dotnet restore
🗄️ Configuração do MySQL
Opção 1: MySQL local
Crie um banco de dados chamado FastWorkshops:

CREATE DATABASE FastWorkshops;
Configure a connection string no arquivo appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=FastWorkshops;User=root;Password=SuaSenha;"
}
Substitua SuaSenha pela senha do seu usuário MySQL.

Opção 2: MySQL via Docker

docker run --name fastworkshops-db -e MYSQL_ROOT_PASSWORD=senha123 -e MYSQL_DATABASE=FastWorkshops -p 3306:3306 -d mysql:8
Atualize a connection string no appsettings.json com a senha usada (senha123).

🚀 Executando a API
No diretório do projeto:

dotnet run
A API será iniciada em:

https://localhost:5001
ou
http://localhost:5000
🔹 A partir daqui, o front-end Angular pode consumir os endpoints disponíveis.

📂 Estrutura do Projeto

FastWorkshops.Api/
│
├─ Controllers/      # Endpoints da API
├─ Data/             # Contexto do Entity Framework
├─ Models/           # Entidades do banco
├─ Migrations/       # Migrações do EF Core
├─ appsettings.json  # Configurações, incluindo connection string
├─ Program.cs        # Configuração do app
└─ README.md         # Documentação
🔧 Boas práticas
Seguir padrão REST para endpoints

Usar Entity Framework Core para gerenciar o banco

Configuração via appsettings.json para diferentes ambientes

Comentários claros e código legível

🤝 Contribuição
Faça um fork do repositório

Crie uma branch para sua feature:


git checkout -b minha-feature
Commit suas alterações:

git commit -m "Descrição da alteração"
Envie para seu fork:

git push origin minha-feature
Abra um Pull Request no repositório principal

📌 Referências
.NET 6 Documentation

Entity Framework Core

MySQL Documentation

Fast Workshops Front-End

# BankMore API 🚀

O **BankMore** é uma API de serviços bancários essenciais (Core Banking) desenvolvida em .NET, focada em alta performance, consistência transacional e resiliência. O projeto foi construído seguindo padrões rigorosos de arquitetura de software para simular o ecossistema real de uma instituição financeira.

---

## 📐 Arquitetura e Padrões de Projeto

O software foi desenhado utilizando conceitos de **DDD (Domain-Driven Design)** para garantir o desacoplamento e a separação de responsabilidades entre as camadas (`Api`, `Application`, `Infrastructure`), além de implementar o padrão **CQRS (Command Query Responsibility Segregation)** através do **MediatR**.

* **Commands e Queries:** Separação absoluta das intenções de escrita e leitura.
* **Alta Performance:** Uso do **Dapper** para persistência e leitura de dados diretamente no banco, garantindo queries limpas, controle transacional e tempo de resposta em milissegundos.
* **Segurança:** Autenticação e autorização via **Token JWT**. Dados sensíveis (como senhas) são protegidos na base com criptografia baseada em **Hash SHA256 e Salt dinâmico**. Além disso, os endpoints trafegam **GUIDs (`idConta`)** como chaves opacas, mitigando a exposição de dados sensíveis na rede (LGPD).

---

## 🛠️ Funcionalidades Implementadas (100% Concluídas)

* **Cadastro de Usuários:** Criação de conta corrente, validação de documento e armazenamento seguro de credenciais.
* **Autenticação (Login):** Validação de credenciais com Salt/Hash e geração de token JWT.
* **Movimentações Financeiras:** Endpoints de Depósito e Saque com validação rígida de saldo em tempo real (retornando HTTP 400 em caso de inconsistência).
* **Transferências entre Contas:** Fluxo atômico que realiza o débito e o crédito de forma casada.
* **Consulta de Saldo e Extrato:** Endpoints RESTful (`/saldo` e `/extrato`) que consolidam a soma de créditos e débitos históricos com conciliação matemática exata e ordenação cronológica.
* **Garantia de Qualidade:** Suíte de **testes unitários** (utilizando **xUnit** e **Moq**) cobrindo as regras de negócio mais críticas, como a lógica dos Handlers e validação de saldo para saques.

---

## 📝 Notas de Transparência e Justificativas Técnicas

Visando a honestidade intelectual e priorização de valor de negócio dentro do tempo de desenvolvimento, foram adotadas as seguintes decisões de arquitetura:

1.  **Resiliência e Idempotência (Time de Crédito):** O sistema está 100% preparado contra falhas de rede no aplicativo através do recebimento de uma `ChaveIdempotencia` (string) no comando de transferência. Caso o aplicativo repita a requisição por perda de conexão, o sistema está blindado para não duplicar o débito. O endpoint opcional de *Tarifas* não foi priorizado.
2.  **Consistência Transacional vs. Microsserviços:** O enunciado sugeria que a API de Transferência realizasse chamadas HTTP externas para processar débitos e créditos, aplicando estornos manuais em caso de falha. Para garantir a integridade dos dados e evitar cenários de saldos órfãos em ambiente de desenvolvimento local, a operação foi centralizada em uma **Transação Atômica única via Dapper** (com `ROLLBACK` automático do banco se uma das pontas falhar).
3.  **Ambiente de Produção (Time de Infraestrutura):** O projeto foi 100% configurado via **Docker Compose** para o ambiente de desenvolvimento local, facilitando a execução imediata pelo avaliador. Por conta do escopo temporal, *não foram gerados os manifestos de implantação do Kubernetes (K8s) nem a configuração física de réplicas*. Contudo, a aplicação foi desenvolvida de forma totalmente **Stateless**, o que significa que ela está nativamente pronta para rodar em múltiplas instâncias orquestradas no Kubernetes sem necessidade de alteração no código.
4.  **Endpoints de Administração:** O endpoint para *Inativar Conta Corrente* não foi priorizado no escopo inicial, concentrando todos os esforços no core financeiro e transacional (depósitos, saques, transferências e extratos).

---

## 🚀 Como Executar o Projeto Localmente

Certifique-se de ter o **Docker** e o **Docker Compose** instalados na sua máquina.

1. Clone o repositório para o seu ambiente local.
2. Na raiz do projeto (onde está o arquivo `docker-compose.yml`), execute o comando abaixo no terminal:

```bash
docker-compose up --build

3.O Docker irá orquestrar a inicialização do banco de dados SQL Server (rodando os scripts de criação de tabelas) e a inicialização da API .NET.

4.Assim que os containers estiverem de pé, acesse o painel do Swagger para realizar os testes:

🌐 Swagger UI: http://localhost:5044/swagger
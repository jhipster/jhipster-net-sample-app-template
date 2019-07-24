using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JHipsterNetSampleApplication.Data;
using JHipsterNetSampleApplication.Models;
using JHipsterNetSampleApplication.Test.Setup;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JHipsterNetSampleApplication.Test.Web.Rest {
    public class OperationResourceIntTest {
        public OperationResourceIntTest()
        {
            _factory = new NhipsterWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();

            _applicationDatabaseContext = _factory.GetRequiredService<ApplicationDatabaseContext>();

            InitTest();
        }

        private readonly DateTime DefaultDate = DateTime.UnixEpoch;
        private readonly DateTime UpdatedDate = DateTime.Now;

        private const string DefaultDescription = "AAAAAAAAAA";
        private const string UpdatedDescription = "BBBBBBBBBB";

        private static readonly decimal DefaultAmount = new decimal(1.0);
        private static readonly decimal UpdatedAmount = new decimal(2.0);

        private readonly NhipsterWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private Operation _operation;

        private Operation CreateEntity()
        {
            return new Operation {
                Date = DefaultDate,
                Description = DefaultDescription,
                Amount = DefaultAmount
            };
        }

        private void InitTest()
        {
            _operation = CreateEntity();
        }

        [Fact]
        public async Task CreateOperation()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Operations.Count();

            // Create the Operation
            var response = await _client.PostAsync("/api/operations", TestUtil.ToJsonContent(_operation));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations.ToList();
            operationList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testOperation = operationList[operationList.Count - 1];
            testOperation.Date.Should().Be(DefaultDate);
            testOperation.Description.Should().Be(DefaultDescription);
            testOperation.Amount.Should().Be(DefaultAmount);
        }

        [Fact]
        public async Task CreateOperationWithExistingId()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Operations.Count();

            // Create the Operation with an existing ID
            _operation.Id = 1L;

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/operations", TestUtil.ToJsonContent(_operation));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations.ToList();
            operationList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CreateOperationWithExistingReferencedEntity()
        {
            // Create a BankAccount to referenced
            var bankAccount = new BankAccount {
                Name = "AAAAAAAAAA",
                Balance = new decimal(1.0)
            };
            _applicationDatabaseContext.BankAccounts.Add(bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeCreate = _applicationDatabaseContext.Operations.Count();

            // Set the referencing field
            _operation.BankAccount = bankAccount;

            // Create the Operation
            var response = await _client.PostAsync("/api/operations", TestUtil.ToJsonContent(_operation));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Operation in the database
            /* AsNoTracking() permits to avoid the use of the cache and force to fetch data from the database.
               It is needed because another context makes the update and our context doesn't have the knowlegde of
               data changes and without it our context will fetch from its cache omitting the changes done. */
            var operationList = _applicationDatabaseContext.Operations
                .Include(operation => operation.BankAccount)
                .AsNoTracking()
                .ToList();
            operationList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testOperation = operationList[operationList.Count - 1];
            testOperation.Date.Should().Be(DefaultDate);
            testOperation.Description.Should().Be(DefaultDescription);
            testOperation.Amount.Should().Be(DefaultAmount);
            testOperation.BankAccount.Should().Be(bankAccount);

            // Validate the BankAccount in the database
            var testBankAccount = await _applicationDatabaseContext.BankAccounts
                .Include(bA => bA.Operations)
                .AsNoTracking()
                .SingleOrDefaultAsync(bA => bA.Id == bankAccount.Id);
            testBankAccount.Name.Should().Be(bankAccount.Name);
            testBankAccount.Balance.Should().Be(bankAccount.Balance);
            testBankAccount.Operations[0].Should().Be(testOperation);
        }

        [Fact]
        public async Task CreateOperationWithManyToManyAssociation()
        {
            // Create a Label to test the ManyToMany association
            var label = new Label {
                Name = "AAAAAAAAAA"
            };
            _applicationDatabaseContext.Labels.Add(label);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeCreate = _applicationDatabaseContext.Operations.Count();

            // Set the referencing field
            _operation.Labels.Add(label);

            // Create the Operation
            var response = await _client.PostAsync("/api/operations", TestUtil.ToJsonContent(_operation));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations
                .Include(operation => operation.OperationLabels)
                    .ThenInclude(operationLabel => operationLabel.Label)
                .AsNoTracking()
                .ToList();
            operationList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testOperation = operationList[operationList.Count - 1];
            testOperation.Date.Should().Be(DefaultDate);
            testOperation.Description.Should().Be(DefaultDescription);
            testOperation.Amount.Should().Be(DefaultAmount);
            testOperation.Labels[0].Should().Be(label);

            // Validate the Label in the database and in particular the Operation referenced            
            var testLabel = await _applicationDatabaseContext.Labels
                .Include(l => l.OperationLabels)
                    .ThenInclude(operationLabel => operationLabel.Operation)
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.Id == 1);
            testLabel.Name.Should().Be(label.Name);
            testLabel.Operations[0].Should().Be(testOperation);
        }

        [Fact]
        public async Task DeleteOperation()
        {
            // Initialize the database
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.Operations.Count();

            var response = await _client.DeleteAsync($"/api/operations/{_operation.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var operationList = _applicationDatabaseContext.Operations.ToList();
            operationList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public async Task DeleteOperationWithExistingReferencedEntity()
        {
            // Create a BankAccount to referenced
            var bankAccount = new BankAccount {
                Name = "AAAAAAAAAA",
                Balance = new decimal(1.0)
            };
            _applicationDatabaseContext.BankAccounts.Add(bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Set the referencing field
            _operation.BankAccount = bankAccount;

            // Initialize the database
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.Operations.Count();

            var response = await _client.DeleteAsync($"/api/operations/{_operation.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var operationList = _applicationDatabaseContext.Operations
                .AsNoTracking()
                .ToList();
            operationList.Count().Should().Be(databaseSizeBeforeDelete - 1);

            // Validate the BankAccount in the database and in particular there is no more Operation referenced
            var testBankAccount = await _applicationDatabaseContext.BankAccounts
                .Include(bA => bA.Operations)
                .AsNoTracking()
                .SingleOrDefaultAsync(bA => bA.Id == bankAccount.Id);
            testBankAccount.Name.Should().Be(bankAccount.Name);
            testBankAccount.Balance.Should().Be(bankAccount.Balance);
            testBankAccount.Operations.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteOperationWithManyToManyAssociation()
        {
            // Create a Label to test the ManyToMany association
            var label = new Label {
                Name = "AAAAAAAAAA"
            };
            _applicationDatabaseContext.Labels.Add(label);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Set the referencing field
            _operation.Labels.Add(label);

            // Initialize the database
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.Operations.Count();

            var response = await _client.DeleteAsync($"/api/operations/{_operation.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var operationList = _applicationDatabaseContext.Operations
                .AsNoTracking()
                .ToList();
            operationList.Count().Should().Be(databaseSizeBeforeDelete - 1);

            // Validate the Label in the database and in particular there is no more Operation referenced
            var testLabel = await _applicationDatabaseContext.Labels
                .Include(l => l.OperationLabels)
                    .ThenInclude(operationLabel => operationLabel.Operation)
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.Id == label.Id);
            testLabel.Name.Should().Be(label.Name);
            testLabel.Operations.Should().BeEmpty();
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Operation));
            var operation1 = new Operation {
                Id = 1L
            };
            var operation2 = new Operation {
                Id = operation1.Id
            };
            operation1.Should().Be(operation2);
            operation2.Id = 2L;
            operation1.Should().NotBe(operation2);
            operation1.Id = 0;
            operation1.Should().NotBe(operation2);
        }

        [Fact]
        public async Task GetAllOperations()
        {
            // Initialize the database
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get all the operationList
            var response = await _client.GetAsync("/api/operations?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_operation.Id);
            json.SelectTokens("$.[*].date").Should().Contain(DefaultDate);
            json.SelectTokens("$.[*].description").Should().Contain(DefaultDescription);
            json.SelectTokens("$.[*].amount").Should().Contain(DefaultAmount);
        }

        [Fact]
        public async Task GetNonExistingOperation()
        {
            var response = await _client.GetAsync("/api/operations/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetOperation()
        {
            // Initialize the database
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get the operation
            var response = await _client.GetAsync($"/api/operations/{_operation.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_operation.Id);
            json.SelectTokens("$.date").Should().Contain(DefaultDate);
            json.SelectTokens("$.description").Should().Contain(DefaultDescription);
            json.SelectTokens("$.amount").Should().Contain(DefaultAmount);
        }

        [Fact]
        public async Task UpdateNonExistingOperation()
        {
            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Operations.Count();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/operations", TestUtil.ToJsonContent(_operation));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations.ToList();
            operationList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task UpdateOperation()
        {
            // Initialize the database
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Operations.Count();

            // Update the operation
            var updatedOperation =
                await _applicationDatabaseContext.Operations.SingleOrDefaultAsync(it => it.Id == _operation.Id);
            // Disconnect from session so that the updates on updatedOperation are not directly saved in db
//TODO detach
            updatedOperation.Date = UpdatedDate;
            updatedOperation.Description = UpdatedDescription;
            updatedOperation.Amount = UpdatedAmount;

            var response = await _client.PutAsync("/api/operations", TestUtil.ToJsonContent(updatedOperation));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations.ToList();
            operationList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testOperation = operationList[operationList.Count - 1];
            testOperation.Date.Should().Be(UpdatedDate);
            testOperation.Description.Should().Be(UpdatedDescription);
            testOperation.Amount.Should().Be(UpdatedAmount);
        }

        [Fact]
        public async Task UpdateOperationWithExistingReferencedEntity()
        {
            // Create two BankAccounts to referenced
            var bankAccount = new BankAccount {
                Name = "AAAAAAAAAA",
                Balance = new decimal(1.0)
            };
            var updatedBankAccount = new BankAccount {
                Name = "BBBBBBBBBB",
                Balance = new decimal(2.0)
            };
            _applicationDatabaseContext.BankAccounts.Add(bankAccount);
            _applicationDatabaseContext.BankAccounts.Add(updatedBankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Set the referencing field
            _operation.BankAccount = bankAccount;

            // Initialize the database with an operation
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Operations.Count();

            // Update the operation
            var updatedOperation = await _applicationDatabaseContext.Operations
                .SingleOrDefaultAsync(it => it.Id == _operation.Id);
            // Disconnect from session so that the updates on updatedOperation are not directly saved in db
//TODO detach
            updatedOperation.Date = UpdatedDate;
            updatedOperation.Description = UpdatedDescription;
            updatedOperation.Amount = UpdatedAmount;
            updatedOperation.BankAccount = updatedBankAccount;

            var response = await _client.PutAsync("/api/operations", TestUtil.ToJsonContent(updatedOperation));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations
                .Include(operation => operation.BankAccount)
                .AsNoTracking()
                .ToList();
            operationList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testOperation = operationList[operationList.Count - 1];
            testOperation.Date.Should().Be(UpdatedDate);
            testOperation.Description.Should().Be(UpdatedDescription);
            testOperation.Amount.Should().Be(UpdatedAmount);
            testOperation.BankAccount.Should().Be(updatedBankAccount);

            // Validate the updatedBankAccount in the database
            var testUpdatedBankAccount = await _applicationDatabaseContext.BankAccounts
                .Include(bA => bA.Operations)
                .AsNoTracking()
                .SingleOrDefaultAsync(bA => bA.Id == updatedBankAccount.Id);
            testUpdatedBankAccount.Name.Should().Be(updatedBankAccount.Name);
            testUpdatedBankAccount.Balance.Should().Be(updatedBankAccount.Balance);
            testUpdatedBankAccount.Operations[0].Should().Be(testOperation);

            // Validate the bankAccount in the database
            var testBankAccount = await _applicationDatabaseContext.BankAccounts
                .Include(bA => bA.Operations)
                .AsNoTracking()
                .SingleOrDefaultAsync(bA => bA.Id == bankAccount.Id);
            testBankAccount.Name.Should().Be(bankAccount.Name);
            testBankAccount.Balance.Should().Be(bankAccount.Balance);
            testBankAccount.Operations.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateOperationWithManyToManyAssociation()
        {
            // Create two Labels to test the ManyToMany association
            var label = new Label {
                Name = "AAAAAAAAAA"
            };
            var updatedLabel = new Label {
                Name = "BBBBBBBBBB"
            };
            _applicationDatabaseContext.Labels.Add(label);
            _applicationDatabaseContext.Labels.Add(updatedLabel);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Set the referencing field
            _operation.Labels.Add(label);

            // Initialize the database with an operation
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Operations.Count();

            // Update the operation
            var updatedOperation = await _applicationDatabaseContext.Operations
                .SingleOrDefaultAsync(it => it.Id == _operation.Id);
            // Disconnect from session so that the updates on updatedOperation are not directly saved in db
//TODO detach
            updatedOperation.Date = UpdatedDate;
            updatedOperation.Description = UpdatedDescription;
            updatedOperation.Amount = UpdatedAmount;
            updatedOperation.Labels.Clear();
            updatedOperation.Labels.Add(updatedLabel);

            var response = await _client.PutAsync("/api/operations", TestUtil.ToJsonContent(updatedOperation));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations
                .Include(operation => operation.OperationLabels)
                    .ThenInclude(operationLabel => operationLabel.Label)
                .AsNoTracking()
                .ToList();
            operationList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testOperation = operationList[operationList.Count - 1];
            testOperation.Date.Should().Be(UpdatedDate);
            testOperation.Description.Should().Be(UpdatedDescription);
            testOperation.Amount.Should().Be(UpdatedAmount);
            testOperation.Labels[0].Should().Be(updatedLabel);

            // Validate the updatedLabel in the database and in particular the Operation referenced
            var testUpdatedLabel = await _applicationDatabaseContext.Labels
                .Include(l => l.OperationLabels)
                    .ThenInclude(operationLabel => operationLabel.Operation)
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.Id == updatedLabel.Id);
            testUpdatedLabel.Name.Should().Be(updatedLabel.Name);
            testUpdatedLabel.Operations[0].Should().Be(testOperation);

            // Validate the label in the database and in particular there is no more Operation referenced
            var testLabel = await _applicationDatabaseContext.Labels
                .Include(l => l.OperationLabels)
                    .ThenInclude(operationLabel => operationLabel.Operation)
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.Id == label.Id);
            testLabel.Name.Should().Be(label.Name);
            testLabel.Operations.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateOperationWithReferencedEntityToNull()
        {
            // Create a BankAccount to referenced
            var bankAccount = new BankAccount {
                Name = "AAAAAAAAAA",
                Balance = new decimal(1.0)
            };
            _applicationDatabaseContext.BankAccounts.Add(bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Set the referencing field
            _operation.BankAccount = bankAccount;

            // Initialize the database with an operation
            _applicationDatabaseContext.Operations.Add(_operation);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Operations.Count();

            // Update the operation
            var updatedOperation = await _applicationDatabaseContext.Operations
                .SingleOrDefaultAsync(it => it.Id == _operation.Id);
            // Disconnect from session so that the updates on updatedOperation are not directly saved in db
//TODO detach
            updatedOperation.Date = UpdatedDate;
            updatedOperation.Description = UpdatedDescription;
            updatedOperation.Amount = UpdatedAmount;
            updatedOperation.BankAccount = null;

            var response = await _client.PutAsync("/api/operations", TestUtil.ToJsonContent(updatedOperation));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Operation in the database
            var operationList = _applicationDatabaseContext.Operations
                .Include(operation => operation.BankAccount)
                .AsNoTracking()
                .ToList();
            operationList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testOperation = operationList[operationList.Count - 1];
            testOperation.Date.Should().Be(UpdatedDate);
            testOperation.Description.Should().Be(UpdatedDescription);
            testOperation.Amount.Should().Be(UpdatedAmount);
            testOperation.BankAccount.Should().BeNull();

            // Validate the bankAccount in the database
            var testBankAccount = await _applicationDatabaseContext.BankAccounts
                .Include(bA => bA.Operations)
                .AsNoTracking()
                .SingleOrDefaultAsync(bA => bA.Id == bankAccount.Id);
            testBankAccount.Name.Should().Be(bankAccount.Name);
            testBankAccount.Balance.Should().Be(bankAccount.Balance);
            testBankAccount.Operations.Should().BeEmpty();
        }
    }
}

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
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Operation));
            var operation1 = new User {
                Id = "operation-1"
            };
            var operation2 = new User {
                Id = operation1.Id
            };
            operation1.Should().Be(operation2);
            operation2.Id = "operation-2";
            operation1.Should().NotBe(operation2);
            operation1.Id = null;
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
    }
}

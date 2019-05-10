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
    public class BankAccountResourceIntTest {
        public BankAccountResourceIntTest()
        {
            _factory = new NhipsterWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();

            _applicationDatabaseContext = _factory.GetRequiredService<ApplicationDatabaseContext>();

            InitTest();
        }

        private const string DefaultName = "AAAAAAAAAA";
        private const string UpdatedName = "BBBBBBBBBB";

        private static readonly decimal DefaultBalance = new decimal(1.0);
        private static readonly decimal UpdatedBalance = new decimal(2.0);

        private readonly NhipsterWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private BankAccount _bankAccount;

        private BankAccount CreateEntity()
        {
            return new BankAccount {
                Name = DefaultName,
                Balance = DefaultBalance
            };
        }

        private void InitTest()
        {
            _bankAccount = CreateEntity();
        }

        [Fact]
        public async Task CheckNameIsRequired()
        {
            var databaseSizeBeforeTest = _applicationDatabaseContext.BankAccounts.Count();

            // set the field null
            _bankAccount.Name = null;

            // Create the BankAccount, which fails.
            var response = await _client.PostAsync("/api/bank-accounts", TestUtil.ToJsonContent(_bankAccount));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var bankAccountList = _applicationDatabaseContext.BankAccounts.ToList();
            bankAccountList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task CreateBankAccount()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.BankAccounts.Count();

            // Create the BankAccount
            var response = await _client.PostAsync("/api/bank-accounts", TestUtil.ToJsonContent(_bankAccount));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the BankAccount in the database
            var bankAccountList = _applicationDatabaseContext.BankAccounts.ToList();
            bankAccountList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testBankAccount = bankAccountList[bankAccountList.Count - 1];
            testBankAccount.Name.Should().Be(DefaultName);
            testBankAccount.Balance.Should().Be(DefaultBalance);
        }

        [Fact]
        public async Task CreateBankAccountWithExistingId()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.BankAccounts.Count();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the BankAccount with an existing ID
            _bankAccount.Id = 1L;

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/bank-accounts", TestUtil.ToJsonContent(_bankAccount));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the BankAccount in the database
            var bankAccountList = _applicationDatabaseContext.BankAccounts.ToList();
            bankAccountList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CreateBankAccountWithExistingReferencedEntity()
        {
            // Get a User to referenced
            var user = _applicationDatabaseContext.Users.ToList()[0];

            var databaseSizeBeforeCreate = _applicationDatabaseContext.BankAccounts.Count();

            // Set the referencing field
            _bankAccount.User = user;

            // Create the BankAccount
            var response = await _client.PostAsync("/api/bank-accounts", TestUtil.ToJsonContent(_bankAccount));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the BankAccount in the database
            var bankAccountList = _applicationDatabaseContext.BankAccounts.ToList();
            bankAccountList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testBankAccount = bankAccountList[bankAccountList.Count - 1];
            testBankAccount.Name.Should().Be(DefaultName);
            testBankAccount.Balance.Should().Be(DefaultBalance);
            testBankAccount.User.Should().Be(user);
        }

        [Fact]
        public async Task DeleteBankAccount()
        {
            // Initialize the database
            _applicationDatabaseContext.BankAccounts.Add(_bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.BankAccounts.Count();

            var response = await _client.DeleteAsync($"/api/bank-accounts/{_bankAccount.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var bankAccountList = _applicationDatabaseContext.BankAccounts.ToList();
            bankAccountList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(BankAccount));
            var bankAccount1 = new User {
                Id = "bankAccount-1"
            };
            var bankAccount2 = new User {
                Id = bankAccount1.Id
            };
            bankAccount1.Should().Be(bankAccount2);
            bankAccount2.Id = "bankAccount-2";
            bankAccount1.Should().NotBe(bankAccount2);
            bankAccount1.Id = null;
            bankAccount1.Should().NotBe(bankAccount2);
        }

        [Fact]
        public async Task GetAllBankAccounts()
        {
            // Initialize the database
            _applicationDatabaseContext.BankAccounts.Add(_bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get all the bankAccountList
            var response = await _client.GetAsync("/api/bank-accounts?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_bankAccount.Id);
            json.SelectTokens("$.[*].name").Should().Contain(DefaultName);
            json.SelectTokens("$.[*].balance").Should().Contain(DefaultBalance);
        }

        [Fact]
        public async Task GetBankAccount()
        {
            // Initialize the database
            _applicationDatabaseContext.BankAccounts.Add(_bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get the bankAccount
            var response = await _client.GetAsync($"/api/bank-accounts/{_bankAccount.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_bankAccount.Id);
            json.SelectTokens("$.name").Should().Contain(DefaultName);
            json.SelectTokens("$.balance").Should().Contain(DefaultBalance);
        }

        [Fact]
        public async Task GetNonExistingBankAccount()
        {
            var response = await _client.GetAsync("/api/bank-accounts/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateBankAccount()
        {
            // Initialize the database
            _applicationDatabaseContext.BankAccounts.Add(_bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.BankAccounts.Count();

            // Update the bankAccount
            var updatedBankAccount =
                await _applicationDatabaseContext.BankAccounts.SingleOrDefaultAsync(it => it.Id == _bankAccount.Id);
            // Disconnect from session so that the updates on updatedBankAccount are not directly saved in db
//TODO detach
            updatedBankAccount.Name = UpdatedName;
            updatedBankAccount.Balance = UpdatedBalance;

            var response = await _client.PutAsync("/api/bank-accounts", TestUtil.ToJsonContent(updatedBankAccount));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the BankAccount in the database
            var bankAccountList = _applicationDatabaseContext.BankAccounts.ToList();
            bankAccountList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testBankAccount = bankAccountList[bankAccountList.Count - 1];
            testBankAccount.Name.Should().Be(UpdatedName);
            testBankAccount.Balance.Should().Be(UpdatedBalance);
        }

        [Fact]
        public async Task UpdateNonExistingBankAccount()
        {
            var databaseSizeBeforeUpdate = _applicationDatabaseContext.BankAccounts.Count();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/bank-accounts", TestUtil.ToJsonContent(_bankAccount));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the BankAccount in the database
            var bankAccountList = _applicationDatabaseContext.BankAccounts.ToList();
            bankAccountList.Count().Should().Be(databaseSizeBeforeUpdate);
        }
    }
}

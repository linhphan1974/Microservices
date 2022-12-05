using BookOnline.Basket.Api.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StackExchange.Redis;

namespace Basket.Api.FuncTest
{
    public class Tests
    {
        private HttpClient _httpClient;
        private Redis
        [SetUp]
        public void Setup()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [Test]
        public void Connect_To_Redis_Success()
        {
            var redis = 

            Assert.Pass();
        }
    }
}
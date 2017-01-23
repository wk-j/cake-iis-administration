using System;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Cake.IISAdministration.Tests
{
    public class IISSpec
    {
        IISManage iis = new IISManage(new IISManageOptions
        {
            UserName = "Administrator",
            Password = Environment.GetEnvironmentVariable("iis"),
            Url = "https://192.168.0.109:55539",
            Token = "gdTAeuJ0LcfGrZnumHvyy_YLN4RI4AbAX8eY9UWuJWAEIMoDYoPV7w"
        });

        [Fact]
        public void ShouldGetSites()
        {
            var results = iis.GetWebsites();
            results.Success.Should().BeTrue();
            results.Data.Count().Should().BeGreaterThan(0);

        }

        [Fact]
        public void ShouldCreateSite()
        {
            var name = "Test-IIS-Administrator";
            var newSite = new NewSite
            {
                Name = name,
                PhysicalPath = $"D:\\IISApplication\\Tests\\{name}",
                Port = 9020
            };

            var result = iis.CreateSite(newSite);
            result.Success.Should().BeTrue();
            result.Data.Name.Should().Be(name);
        }
    }
}
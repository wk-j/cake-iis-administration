using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Cake.IISAdministration.Tests
{
    public class IISSpec
    {

        [Fact]
        public void ShouldGetSites()
        {
            var iis = new IISManage(new IISManageOptions {
                UserName = "Administrator",
                Password = Environment.GetEnvironmentVariable("iis"),
                Url = "https://192.168.0.109:55539",
                Token = "gdTAeuJ0LcfGrZnumHvyy_YLN4RI4AbAX8eY9UWuJWAEIMoDYoPV7w"
            });

            var sites = iis.GetWebsites();
            sites.Count().Should().BeGreaterThan(0);

        }
    }
}

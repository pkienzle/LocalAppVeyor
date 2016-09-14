﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using LocalAppVeyor.Engine.Configuration;
using LocalAppVeyor.Engine.Configuration.Reader;
using Xunit;

namespace LocalAppVeyor.Engine.UnitTests
{
    public class ConfigurationYamlReading
    {
        [Fact]
        public void ShouldReadEnvironmentWithCommonAndMatrixVariables()
        {
            const string yaml = @"
environment:
  common_var1: common_value1
  common_var2:
    secure: common_value2_secured
  matrix:
    - db: mysql
      password: mysql_password
    - db: sqlserver
      password:
        secure: sqlserver_secured_password
";

            var conf = new BuildConfigurationYamlStringReader(yaml).GetBuildConfiguration();

            conf.EnvironmentVariables.ShouldBeEquivalentTo(
                new EnvironmentVariables(
                    new List<Variable>
                    {
                        new Variable("common_var1", "common_value1", false),
                        new Variable("common_var2", "common_value2_secured", true)
                    },
                    new List<ReadOnlyCollection<Variable>>
                    {
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "mysql", false),
                            new Variable("password", "mysql_password", false)
                        }),
                        new ReadOnlyCollection<Variable>(new List<Variable>
                        {
                            new Variable("db", "sqlserver", false),
                            new Variable("password", "sqlserver_secured_password", true)
                        })
                    }));
        }

        [Fact]
        public void ShouldReadNoEnvironmentButPropertiesShouldNotBeNullOnlyEmpty()
        {
            var conf = new BuildConfigurationYamlStringReader("").GetBuildConfiguration();

            conf.EnvironmentVariables.Should().NotBeNull();
            conf.EnvironmentVariables.CommonVariables.Should().BeEmpty();
            conf.EnvironmentVariables.Matrix.Should().BeEmpty();
        }
    }
}
using Shouldly;
using Xunit;

namespace ADatabaseFixture.Tests;

public class SqlServerDatabaseAdapterTests
{
    [Fact]
    public void Can_create_ConnectionString()
    {
        var adapter = new SqlServerDatabaseAdapter(_ => null!);

        adapter
            .GetDatabaseConnectionString()
            .ShouldBe($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={adapter.DatabaseName};Integrated Security=True");
    }

    [Fact]
    public void Can_create_MasterConnectionString()
    {
        var adapter = new SqlServerDatabaseAdapter(_ => null!);

        adapter
            .GetMasterConnectionString()
            .ShouldBe($"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
    }
}

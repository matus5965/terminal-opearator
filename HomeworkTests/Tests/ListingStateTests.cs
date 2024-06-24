using HomeworkTests.Exceptions;
using InformationSystemHZS;

namespace HomeworkTests.Tests;

[TestFixture]
public class ListingStateTests
{
    [SetUp]
    public void Setup()
    {
        // Intentionally empty
    }

    [Test]
    public void InitialStationsListing()
    {
        var tester = new ConsoleTesterContainAll(new string[] { "list-stations" },
            new string[]
            {
                "S01#Brnoslava - Královnino pole#3#(84, 78)", "S02#Brnoslava - Staré město#3#(10, 18)",
                "S03#Brnoslava - Venkov#4#(92, 18)"
            });

        try
        {
            Runner.Main(tester);
        }
        catch (InputEndException e)
        {
            if (tester.HasValidEndingState())
            {
                Assert.Pass();
            }

            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.Fail();
        }

        Assert.Fail();
    }

    [Test]
    public void InitialUnitsListing()
    {
        var tester = new ConsoleTesterContainAll(new string[] { "list-units" },
            new string[]
            {
                "S01#J01#Scania P440 6x6#5/5#AVAILABLE",
                "S01#J02#Toyota Hilux#4/4#AVAILABLE",
                "S01#J03#Tatra Force 6x6#3/3#AVAILABLE",
                "S02#J01#Scania P440 6x6#4/5#AVAILABLE",
                "S02#J02#Volkswagen Crafter 4x4#2/4#AVAILABLE",
                "S02#J03#Terex Demag AC40#3/3#AVAILABLE",
                "S03#J01#Scania P450 6x6#5/5#AVAILABLE",
                "S03#J02#Scania P450 6x6#4/5#AVAILABLE",
                "S03#J03#Iveco Daily 4x4#2/3#AVAILABLE",
                "S03#J04#Scania G500#3/3#AVAILABLE"
            });

        try
        {
            Runner.Main(tester);
        }
        catch (InputEndException e)
        {
            if (tester.HasValidEndingState())
            {
                Assert.Pass();
            }

            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.Fail();
        }

        Assert.Fail();
    }

    [Test]
    public void InitialIncidentsListing()
    {
        var tester = new ConsoleTesterContainAll(new string[] { "list-incidents" }, new string[] { });

        try
        {
            Runner.Main(tester);
        }
        catch (InputEndException e)
        {
            if (tester.HasValidEndingState())
            {
                Assert.Pass();
            }

            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.Fail();
        }

        Assert.Fail();
    }
}
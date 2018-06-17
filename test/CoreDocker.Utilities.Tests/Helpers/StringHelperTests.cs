using CoreDocker.Utilities.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Utilities.Tests.Helpers
{
    [TestFixture]
    public class StringHelperTests
    {
        [Test]
        public void UriCombine_GivenBaseUrlAndAdd_ShouldResultInValidUrl()
        {
            // arrange
            var baseUrl = "http://sample";
            // action
            var uriCombine = baseUrl.UriCombine("t","tt/","i.html");
            // assert
            uriCombine.Should().Be("http://sample/t/tt/i.html");
        }
        
        [Test]
        public void ToInitialCase_GivenValue_ShouldResultValidResult()
        {
            // arrange
            var baseUrl = "rolf sample";
            // action
            var uriCombine = baseUrl.ToInitialCase();
            // assert
            uriCombine.Should().Be("Rolf sample");
        }

        [Test]
        public void UnderScoreAndCamelCaseToHumanReadable_GivenValue_ShouldResultValidResult()
        {
            // arrange
            var baseUrl = "rolf sample_asRample";
            // action
            var uriCombine = baseUrl.UnderScoreAndCamelCaseToHumanReadable();
            // assert
            uriCombine.Should().Be("rolf sample as Rample");
        }


        [Test]
        public void GetEmailAddresses_GivenValue_ShouldResultValidResult()
        {
            // arrange
            var baseUrl = "rolf@f.com,rolf@fd.com rolffd";
            // action
            var uriCombine = baseUrl.GetEmailAddresses();
            // assert
            uriCombine.Should()
                .Contain("rolf@f.com")
                .And.Contain("rolf@fd.com")
                .And.HaveCount(2);
        }

        [Test]
        public void StripHtml_GivenHtml_ShouldRemoveTags()
        {
            // arrange
            var baseUrl = "<b>Rolf<strong></string></b>";
            // action
            var uriCombine = baseUrl.StripHtml();
            // assert
            uriCombine.Should().Be("Rolf");
        }


        [Test]
        public void EnsureDoesNotStartWith_GivenStartingValue_ShouldRemoveValue()
        {
            // arrange
            var baseUrl = "/test";
            // action
            var uriCombine = baseUrl.EnsureDoesNotStartWith("/");
            // assert
            uriCombine.Should().Be("test");
        }

        [Test]
        public void EnsureEndsWith_GivenStartingValue_ShouldRemoveValue()
        {
            // arrange
            var baseUrl = "/test";
            // action
            var uriCombine = baseUrl.EnsureEndsWith("/");
            // assert
            uriCombine.Should().Be("/test/");
        }


        [Test]
        public void Mask_GivenGivenLongValue_ShouldMask()
        {
            // arrange
            // action
            var uriCombine = "casdcasdcasd".Mask(5);
            // assert
            uriCombine.Should().Be("casdcXXXX");
        }

        [Test]
        public void Mask_GivenGivenEmpty_ShouldMask()
        {
            // arrange
            // action
            var uriCombine = "".Mask(5);
            // assert
            uriCombine.Should().Be("XXXX");
        }

        [Test]
        public void Mask_GivenGivenNull_ShouldMask()
        {
            // arrange
            // action
            var uriCombine = StringHelper.Mask(null, 5);
            // assert
            uriCombine.Should().Be(null);
        }


    }
}
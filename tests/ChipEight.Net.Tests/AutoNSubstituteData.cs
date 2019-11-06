using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace ChipEight.Net.Tests
{
    class AutoNSubstituteData : AutoDataAttribute
    {
        public AutoNSubstituteData()
            : base(() => new Fixture()
                .Customize(new AutoNSubstituteCustomization()))
        {

        }
    }
}

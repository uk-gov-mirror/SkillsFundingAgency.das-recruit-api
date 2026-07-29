using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Models.Mappers;

namespace SFA.DAS.Recruit.Api.UnitTests.Extensions;

internal class WhenMappingPatchRequest
{
    [Test, MoqAutoData]
    public void To_Entity_Then_The_Entity_Is_Mapped()
    {
        // arrange
        var source = new JsonPatchDocument<ApplicationReview>();
        source.Replace(x => x.AccountId, 1234);
        
        // act
        var result = source.ToEntity();

        // assert
        result.Operations.Should().HaveCount(1);
        result.Operations[0].op.Should().Be(source.Operations[0].op);
        result.Operations[0].path.Should().Be(source.Operations[0].path);
        result.Operations[0].value.Should().Be(source.Operations[0].value);
        result.Operations[0].from.Should().Be(source.Operations[0].from);
    }
}

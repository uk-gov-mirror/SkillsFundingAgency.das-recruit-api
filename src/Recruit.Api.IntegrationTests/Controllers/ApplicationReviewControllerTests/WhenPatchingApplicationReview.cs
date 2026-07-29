using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.UnitTests;
using SFA.DAS.Recruit.Contracts.ApiRequests;
using SFA.DAS.Recruit.Contracts.ApiResponses;

namespace SFA.DAS.Recruit.Api.IntegrationTests.Controllers.ApplicationReviewControllerTests;

public class WhenPatchingApplicationReview : BaseFixture
{
    [Test]
    public async Task Then_NotFound_When_Application_Review_Does_Not_Exist()
    {
        // arrange
        Server.DataContext.Setup(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(Fixture.CreateMany<ApplicationReviewEntity>(5).ToList());

        var patchDocument = new JsonPatchDocument<ApplicationReview>();
        patchDocument.Replace(x => x.Status, ApplicationReviewStatus.Successful);

        // act
        var response = await Client.PatchAsJsonAsync(
            new PatchApplicationreviewsByApplicationIdApiRequest { ApplicationId = Guid.NewGuid() }.PatchUrl,
            patchDocument);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Then_ApplicationReview_Is_Patched_Successfully()
    {
        // arrange
        var items = Fixture.CreateMany<ApplicationReviewEntity>(5).ToList();
        var itemsClone = items.JsonClone();
        var target = itemsClone[2];

        Server.DataContext
            .SetupSequence(x => x.ApplicationReviewEntities)
            .ReturnsDbSet(items)
            .ReturnsDbSet(itemsClone);

        Server.DataContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var patchDocument = new JsonPatchDocument<ApplicationReview>();
        patchDocument.Replace(x => x.DateSharedWithEmployer, DateTime.UtcNow);

        // act
        var response = await Client.PatchAsJsonAsync(
            new PatchApplicationreviewsByApplicationIdApiRequest { ApplicationId = target.Id }.PatchUrl,
            patchDocument);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Server.DataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

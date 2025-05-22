using LivenerTechTest.interfaces;
using LivenerTechTest.types;
using Microsoft.AspNetCore.Mvc;

namespace LivenerTechTest;

[ApiController]
[Route("loganalysis")]
public class LogController(IDataStore dataStore) : ControllerBase
{
    public ActionResult<LogAggregateDto> Index()
    {
        return dataStore.GetLogAggregate();
    }
}
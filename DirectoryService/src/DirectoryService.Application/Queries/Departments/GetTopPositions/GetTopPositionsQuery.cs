using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.Departments.GetTopPositions;

public record GetTopPositionsQuery(int Limit = 5) : IQuery;
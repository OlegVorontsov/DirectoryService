namespace DirectoryService.Application.Shared.DTOs;

public record FilteredListDTO<T>(int Page, int Size, IEnumerable<T> Data, int Total);
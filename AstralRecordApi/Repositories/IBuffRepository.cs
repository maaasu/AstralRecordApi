using AstralRecordApi.Models;

namespace AstralRecordApi.Repositories;

public interface IBuffRepository
{
    BuffResponse? GetById(string buffId);
}
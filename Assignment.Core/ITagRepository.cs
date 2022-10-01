namespace Assignment.Core;

public interface ITagRepository
{
    (Response Response, int TagId) Create(TagCreateDTO tag);
    IReadOnlyCollection<TagDTO> Read();
    TagDTO Find(int tagId);
    Response Update(TagUpdateDTO tag);
    Response Delete(int tagId, bool force = false);
}
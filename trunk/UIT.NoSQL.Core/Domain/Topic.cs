using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIT.NoSQL.Core.Domain
{
    public interface ITopicObjectDocument
    {
        string Id { get; set; }
        string TopicName { get; set; }
        DenormalizedUser<UserObject> CreateBy { get; set; }
        DateTime LastModified { get; set; }
        UInt32 NumberOfView { get; set; }
        UInt32 NumberOfComment { get; set; }
        bool isDeleted { get; set; }
        List<UserTopic> ListUserTopic { get; set; }
    }

    public class TopicObject : ITopicObjectDocument
    {
        public string Id { get; set; }
        public string TopicName { get; set; }
        public string Content { get; set; }
        public DenormalizedUser<UserObject> CreateBy { get; set; }
        public string GroupId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModified { get; set; }
        public UInt32 NumberOfView { get; set; }
        public UInt32 NumberOfComment { get; set; }
        public bool isDeleted { get; set; }

        public List<CommentObject> ListComment { get; set; }
        public List<FileAttach> ListFilesAttach { get; set; }
        public List<UserTopic> ListUserTopic { get; set; }

        public TopicObject()
        {
            ListComment = new List<CommentObject>();
            ListFilesAttach = new List<FileAttach>();
            ListUserTopic = new List<UserTopic>();
        }

        public UInt32 GetNumberOfComment()
        {
            return (UInt32)ListComment.Count();
        }

    }

    public class DenormalizedTopic<T> where T : ITopicObjectDocument
    {
        public string Id { get; set; }
        public string TopicName { get; set; }
        public DenormalizedUser<UserObject> CreateBy { get; set; }
        public DateTime LastModified { get; set; }
        public UInt32 NumberOfView { get; set; }
        public UInt32 NumberOfComment { get; set; }
        public bool isDeleted { get; set; }
        public List<UserTopic> ListUserTopic { get; set; }

        public static implicit operator DenormalizedTopic<T>(T doc)
        {
            return new DenormalizedTopic<T>
            {
                Id = doc.Id,
                TopicName = doc.TopicName,
                CreateBy = doc.CreateBy,
                LastModified = doc.LastModified,
                NumberOfView = doc.NumberOfView,
                NumberOfComment = doc.NumberOfComment,
                isDeleted = doc.isDeleted,
                ListUserTopic = doc.ListUserTopic
            };
        }
    }

    public class CommentObject
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string ParentContent { get; set; }
        public DenormalizedUser<UserObject> CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public bool isDeleted { get; set; }
        public List<FileAttach> ListFilesAttach { get; set; }

        public CommentObject()
        {
            ListFilesAttach = new List<FileAttach>();
        }
    }

    public class FileAttach
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Size { get; set; }
        public string RealName { get; set; }
    }

    public class UserTopic
    {
        public string UserId { get; set; }
        public int NumberOfNewPosts { get; set; }
        public UserTopic()
        {
            NumberOfNewPosts = 0; 
        }
    }
}

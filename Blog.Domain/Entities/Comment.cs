using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string Content { get; set; } = null!;

        public bool IsApproved { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public Post Post { get; set; } = null!;
    }
}

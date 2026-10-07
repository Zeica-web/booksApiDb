using System;
using System.Collections.Generic;

namespace booksApiDb.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int GenreId { get; set; }

    public virtual Genre Genre { get; set; } = null!;
}

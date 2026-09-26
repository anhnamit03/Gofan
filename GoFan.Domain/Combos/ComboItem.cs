using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Domain.Combos;

public class ComboItem
{
    public int Id { get; set; }

    public int ComboId { get; set; }

    public int ProductId { get; set; }

    public DateTime CreatedAt { get; set; }
}
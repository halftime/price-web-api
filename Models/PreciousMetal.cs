using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace price_web_api.Models;

public class PreciousMetal : Investment
{
    // Inherits Id, Symbol, Name from Investment
    // Add any PreciousMetal-specific properties or validation here if needed
}

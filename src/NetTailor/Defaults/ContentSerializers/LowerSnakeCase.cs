﻿using System.Text.Json;
using Microsoft.IO;
using NetTailor.Extensions;

namespace NetTailor.Defaults.ContentSerializers;

public class LowerSnakeCase : AbstractReaderWriter
{
    public LowerSnakeCase(RecyclableMemoryStreamManager memoryStreamManager) : base(memoryStreamManager)
    {
    }

    protected override JsonSerializerOptions Options => JsonSerializerOptionsCache.LowerSnakeCase;
}
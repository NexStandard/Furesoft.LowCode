﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Furesoft.LowCode.Attributes;
using Furesoft.LowCode.Evaluation;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace Furesoft.LowCode.Nodes.IO.Archives;

[NodeCategory("IO/FileSystem")]
[NodeIcon("M0 0H18V4H0V0M1 18V5H17V18H1M11 11V8H7V11H4L9 16 14 11H11Z")]
public class ArchiveNode : InputOutputNode
{
    public ArchiveNode() : base("Compress Directory To Archive")
    {
    }

    [DataMember(EmitDefaultValue = false)]
    [Required]
    public Evaluatable<string> Path { get; set; }

    [Required]
    [DataMember(EmitDefaultValue = false)]
    public Evaluatable<string> OutputFilename { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [Required]
    public ArchiveType Type { get; set; }

    [DataMember(EmitDefaultValue = false)] public string SearchPattern { get; set; } = "*";

    [DataMember(EmitDefaultValue = false)] public SearchOption SearchOption { get; set; }

    public override async Task Execute(CancellationToken cancellationToken)
    {
        await using (var zip = File.OpenWrite(OutputFilename))
        using (var zipWriter = WriterFactory.Open(zip, Type, CompressionType.Deflate))
        {
            zipWriter.WriteAll(Path, SearchPattern, SearchOption);
        }

        await ContinueWith(OutputPin, cancellationToken);
    }
}

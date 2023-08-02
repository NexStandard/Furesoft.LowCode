﻿using Furesoft.LowCode.Designer.Core;

namespace Furesoft.LowCode.Nodes.Network.REST;

public class PostRequest : RestBaseNode, IOutVariableProvider
{
    public string Content { get; set; }
    public string OutVariable { get; set; }
    public PostRequest() : base("POST")
    {
    }

    public override async Task Invoke(CancellationToken cancellationToken)
    {
        var result = await client.PostAsync(Evaluate<string>(URL), new StringContent(Content), cancellationToken);
        
        SetOutVariable(OutVariable, result);
    }
}

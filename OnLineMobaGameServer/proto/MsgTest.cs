//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/MsgTest.proto
namespace PBMessage
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgTest")]
  public partial class MsgTest : global::ProtoBuf.IExtensible
  {
    public MsgTest() {}
    
    private string _protoName = @"MsgTest";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"protoName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(@"MsgTest")]
    public string protoName
    {
      get { return _protoName; }
      set { _protoName = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
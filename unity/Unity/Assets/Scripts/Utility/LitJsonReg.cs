using LitJson;

namespace ET.Utility
{
    public class LitJsonReg
    {
        private static void RegisterOperation()
        {
            void Exporter<T>(Operation<T> obj, JsonWriter writer) where T:JsonCmd
            {
                writer.WriteObjectStart();
                
                writer.WritePropertyName("Op");
                writer.Write(obj.Op);
                writer.WritePropertyName("OpData");
                // writer.Write(obj.OpData);
            }
        }
        
        private static void RegisterUserMove()
        {
            void Exporter(UserMove obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("ts");
                writer.Write(obj.ts);
                writer.WritePropertyName("");
                    
            }
        }

        private static void RegisterUserList()
        {
            void Exporter(UserList obj, JsonWriter writer)
            {
                writer.WriteObjectStart();
                writer.WriteArrayStart();
                foreach (var uInfo in obj.userInfos)
                {
                    writer.Write(JsonMapper.ToJson(uInfo));
                }
                writer.WriteArrayEnd();
                writer.WritePropertyName("");
                
            }
        }
    }
}
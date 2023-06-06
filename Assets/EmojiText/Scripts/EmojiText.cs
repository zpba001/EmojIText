using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using LitJson;

public class EmojiText : Text
{
    private const float ICON_SCALE_OF_DOUBLE_SYMBOLE = 0.85f;
    protected string m_orgStr;                      // 原字符串
    public string m_outputText;                     // 处理后的字符串
    public TextAsset m_atlasCfgFile;                    // 图集的配置
    public static TextAsset s_atlasCfgFile;                    // 图集的配置

    public override float preferredWidth => cachedTextGeneratorForLayout.GetPreferredWidth(m_outputText, GetGenerationSettings(rectTransform.rect.size)) / pixelsPerUnit;
    public override float preferredHeight => cachedTextGeneratorForLayout.GetPreferredHeight(m_outputText, GetGenerationSettings(rectTransform.rect.size)) / pixelsPerUnit;


    protected Dictionary<int, EmojiInfo> m_faceIdxDict = new Dictionary<int, EmojiInfo>();

    // 表情的数据
    public struct EmojiInfo
    {
        public string m_key;            // 表情的key
        public int m_startIdx;          // 表情开始的下标
    }

    readonly UIVertex[] m_TempVerts = new UIVertex[4];
    
    protected override void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>(true);
        if ( canvas != null)
        {
            var v1 = canvas.additionalShaderChannels;
            var v2 = AdditionalCanvasShaderChannels.TexCoord1;
            if ((v1 & v2) != v2)
            {
                canvas.additionalShaderChannels |= v2;
            }
        }
    }
    // 初始化字符串，把能变成表情的字符串替换成"xx"，并且初始化表情的数据
    protected void ResetStr(string str)
    {
        if( FaceAtlasCfg.IsInit() == false)
        {
            if( s_atlasCfgFile == null)
            {
                s_atlasCfgFile = m_atlasCfgFile;
            }
            FaceAtlasCfg.Init(s_atlasCfgFile.text);
        }

        str = str.Replace(" ", "\u00A0");
        if (m_orgStr == str)
        {
            return;
        }
        m_faceIdxDict.Clear();
        m_orgStr = str;
        if (string.IsNullOrEmpty(m_orgStr))
        {
            m_outputText = "";
            return;
        }
        string aimStr = "";
        // 找到所有的[/f.+]
        int nParcedCount = 0;           // 已经解析的表情数量
        int nOffset = 0;                // 下标的偏移量( 源字符串跟目标字符串的差值)
        int endIdx = 0;                 // 上一个表情的结束位置
        string orgStr = m_orgStr;        // 
        if (supportRichText)
        {
            // 如果支持富文本，去掉富文本
            orgStr = ReplaceRichText(m_orgStr);        // 去掉富文本
        }
        // GFun.Log( "去掉富文本"+ orgStr);
        MatchCollection matches = Regex.Matches(orgStr, "\\[/f.+?\\]");
        MatchCollection matches2 = Regex.Matches(m_orgStr, "\\[/f.+?\\]");
        Match tempMatch = null;
        Match tempMatch2 = null;
        for (int i = 0; i < matches.Count; i++)
        {
            tempMatch = matches[i];
            tempMatch2 = matches2[i];
            // 获取表情的key
            string subStr = tempMatch2.Value.Substring(3, tempMatch2.Value.Length - 4);
            // 是否存在这个key
            if (FaceAtlasCfg.IsExistKey(subStr))
            {
                int startIdx = tempMatch.Index - nOffset;
                string tempStr = m_orgStr.Substring(endIdx, tempMatch2.Index - endIdx);      // 要加入目标串的子串
                                                                                             // 替换成"XX"
                aimStr = aimStr + tempStr + "XX";
                endIdx = tempMatch2.Index + tempMatch2.Length;
                nOffset = nOffset + tempMatch2.Length - 2;
                nParcedCount++;

                // 封装进字典中
                EmojiInfo info = new EmojiInfo();
                info.m_key = subStr;
                info.m_startIdx = startIdx;
                m_faceIdxDict.Add(startIdx, info);
                // GFun.Log( aimStr);
            }
        }
        aimStr = aimStr + m_orgStr.Substring(endIdx);
        m_outputText = aimStr;
        // GFun.Log( "==============字符串="+m_outputText);
    }
    /// <summary>
    /// 换掉富文本
    /// </summary>
    private string ReplaceRichText(string str)
    {
        str = Regex.Replace(str, @"<color=(.+?)>(.+?)</color>", "$2");
        // str = str.Replace("</color>", "");
        str = Regex.Replace(str, @"<a href=(.+?)>", "");
        str = str.Replace("</a>", "");
        str = str.Replace("<b>", "");
        str = str.Replace("</b>", "");
        str = str.Replace("<i>", "");
        str = str.Replace("</i>", "");
        str = str.Replace("\n", "");
        str = str.Replace("\t", "");
        str = str.Replace("\r", "");
        str = str.Replace(" ", "");

        return str;
    }
    //         /// <summary>
    //         /// 获取超链接解析后的最后输出文本
    //         /// </summary>
    //         protected virtual string GetOutputText(string outputText)
    //         {
    //             s_TextBuilder.Length = 0;
    //             m_HrefInfos.Clear();

    //             if (string.IsNullOrEmpty(outputText))
    //                 return "";

    //             s_TextBuilder.Remove(0, s_TextBuilder.Length);

    //             int textIndex = 0;
    //             int newIndex = 0;
    //             int removeEmojiCount = 0;

    //             foreach (Match match in m_HrefRegex.Matches(outputText))
    //             {
    //                 var hrefInfo = new HrefInfo();
    //                 string part = outputText.Substring(textIndex, match.Index - textIndex);
    //                 int removeEmojiCountNew = 0;
    //                 MatchCollection collection = m_EmojiRegex.Matches(part);

    //                 foreach (Match emojiMatch in collection)
    //                 {
    //                     removeEmojiCount += 8;
    //                     removeEmojiCountNew += 8;
    //                 }

    //                 s_TextBuilder.Append(part);
    //                 s_TextBuilder.Append("<color=blue>");
    //                 int startIndex = s_TextBuilder.Length * 4 - removeEmojiCount;
    //                 s_TextBuilder.Append(match.Groups[2].Value);
    //                 int endIndex = s_TextBuilder.Length * 4 - removeEmojiCount;
    //                 s_TextBuilder.Append("</color>");

    //                 hrefInfo.startIndex = startIndex;// 超链接里的文本起始顶点索引
    //                 hrefInfo.endIndex = endIndex;

    // #if UNITY_2019_1_OR_NEWER
    //                 newIndex = newIndex + ReplaceRichText(part).Length * 4 - removeEmojiCountNew;//移除超连接前面的表情的顶点
    //                 int newStartIndex = newIndex;
    //                 newIndex = newIndex + match.Groups[2].Value.Length * 4;
    //                 hrefInfo.newStartIndex = newStartIndex;
    //                 hrefInfo.newEndIndex = newIndex;
    // #endif
    //                 hrefInfo.name = match.Groups[1].Value;
    //                 m_HrefInfos.Add(hrefInfo);
    //                 textIndex = match.Index + match.Length;
    //             }

    //             s_TextBuilder.Append(outputText.Substring(textIndex, outputText.Length - textIndex));
    //             return s_TextBuilder.ToString();
    //         }
    // 顶点改变时
    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();

        ResetStr(text);
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if( FaceAtlasCfg.IsInit() == false)
        {
            if( s_atlasCfgFile == null)
            {
                s_atlasCfgFile = m_atlasCfgFile;
            }
            FaceAtlasCfg.Init(s_atlasCfgFile.text);
        }
        // text
        if (font == null)
        {
            return;
        }
        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.Populate(m_outputText, settings);         // 重置网格

        Rect inputRect = rectTransform.rect;

        // get the text alignment anchor point for the text in local space
        Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
        Vector2 refPoint = Vector2.zero;
        refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
        refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);


        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;
        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        int invalidCount = 0;               // 无效的顶点的数量
        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                EmojiInfo info;
                int index = i / 4;
                index = index - invalidCount;
                if (m_faceIdxDict.TryGetValue(index, out info))
                {
                    // 处理表情
                    //compute the distance of '[' and get the distance of emoji 
                    Rect aimRect = FaceAtlasCfg.GetFaceUV(info.m_key);


                    float fCharHeight = verts[i + 1].position.y - verts[i + 2].position.y;
                    float fCharWidth = verts[i + 1].position.x - verts[i].position.x;

                    //计算2个XX的距离
                    float emojiSize = 2 * fCharWidth * ICON_SCALE_OF_DOUBLE_SYMBOLE;

                    float fHeightOffsetHalf = (emojiSize - fCharHeight) * 0.5f;
                    float fStartOffset = emojiSize * ((1 - ICON_SCALE_OF_DOUBLE_SYMBOLE) * 0.5f);        // 开始的偏移位置

                    m_TempVerts[3] = verts[i];//1
                    m_TempVerts[2] = verts[i + 1];//2
                    m_TempVerts[1] = verts[i + 2];//3
                    m_TempVerts[0] = verts[i + 3];//4

                    m_TempVerts[0].position += new Vector3(fStartOffset, -fHeightOffsetHalf, 0);
                    m_TempVerts[1].position += new Vector3(fStartOffset - fCharWidth + emojiSize, -fHeightOffsetHalf, 0);
                    m_TempVerts[2].position += new Vector3(fStartOffset - fCharWidth + emojiSize, fHeightOffsetHalf, 0);
                    m_TempVerts[3].position += new Vector3(fStartOffset, fHeightOffsetHalf, 0);

                    m_TempVerts[0].position *= unitsPerPixel;
                    m_TempVerts[1].position *= unitsPerPixel;
                    m_TempVerts[2].position *= unitsPerPixel;
                    m_TempVerts[3].position *= unitsPerPixel;

                    float pixelOffset = aimRect.width / 32 / 2;         // 好像是为了防止出现边框的问题
                                                                        // float pixelOffset = 0;
                    m_TempVerts[0].uv1 = new Vector2(aimRect.xMin + pixelOffset, aimRect.yMin + pixelOffset);
                    m_TempVerts[1].uv1 = new Vector2(aimRect.xMin - pixelOffset + aimRect.width, aimRect.yMin + pixelOffset);
                    m_TempVerts[2].uv1 = new Vector2(aimRect.xMin - pixelOffset + aimRect.width, aimRect.yMin - pixelOffset + aimRect.height);
                    m_TempVerts[3].uv1 = new Vector2(aimRect.xMin + pixelOffset, aimRect.yMin - pixelOffset + aimRect.height);

                    toFill.AddUIVertexQuad(m_TempVerts);

                    i += 4 * 2 - 1;
                }
                else
                {

                    // 正常的文字
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }

                    if (tempVertsIndex == 0)
                    {
                        // 无效的顶点(富文本可能会出现)
                        float fCharHeight = verts[i + 1].position.y - verts[i + 2].position.y;
                        float fCharWidth = verts[i + 1].position.x - verts[i].position.x;
                        if (fCharWidth == 0 && fCharWidth == 0)
                        {
                            invalidCount++;
                        }
                    }

                }
            }

        }
        m_DisableFontTextureRebuiltCallback = false;
    }
}


// 表情图集的配置
public class FaceFrameCfgInfo
{
    // "frame": {"x":0,"y":0,"w":60,"h":60},
    // "rotated": false,
    // "trimmed": false,
    // "spriteSourceSize": {"x":0,"y":0,"w":60,"h":60},
    // "sourceSize": {"w":60,"h":60},
    // "pivot": {"x":0.5,"y":0.5}
    public string m_cfgId;                              // 配置id
    public int m_x;                                     // x坐标
    public int m_y;                                     // y坐标
    public int m_w;                                     // 宽度
    public int m_h;                                     // 高度


    public FaceFrameCfgInfo()
    {
    }
    public FaceFrameCfgInfo(string cfgId, JsonData jsonData)
    {
        string[] strArr = cfgId.Split('.');
        if (strArr != null && strArr.Length > 0)
        {
            m_cfgId = strArr[0];
            JsonData frameJson = jsonData.GetJsonInJson("frame");
            if (frameJson != null)
            {
                m_x = frameJson.GetIntInJson("x");
                m_y = frameJson.GetIntInJson("y");
                m_w = frameJson.GetIntInJson("w");
                m_h = frameJson.GetIntInJson("h");
            }
        }

    }
}
// 表情图集的配置
public class FaceAtlasCfg
{
    static Vector2 s_texSize;                           // 图片的宽高
    static Dictionary<string, FaceFrameCfgInfo> s_allFaceDict = null;           // 表情的信息

    // 是否初始化
    public static bool IsInit()
    {
        return s_allFaceDict != null;
    } 
    // 初始化
    public static void Init(string cfgStr)
    {
        if (s_allFaceDict == null)
        {
            s_allFaceDict = new Dictionary<string, FaceFrameCfgInfo>();
        }
        s_allFaceDict.Clear();
        JsonData jsonData = JsonMapper.ToObject(cfgStr);
        // 获取图片的宽高
        JsonData metaJson = jsonData.GetJsonInJson("meta");
        s_texSize = metaJson.GetVector2InJson("size", Vector2.zero);
        // 获取所有碎图的信息
        JsonData cfgs = JsonUtility.GetJsonInJson(jsonData, "frames");
        ICollection<string> keys = cfgs.Keys;
        foreach (string key in keys)
        {
            // key1的格式为：f01.png， 只取前面的名字，不要后缀
            FaceFrameCfgInfo info = new FaceFrameCfgInfo(key, cfgs[key]);
            if (!string.IsNullOrEmpty(info.m_cfgId))
            {
                s_allFaceDict.Add(info.m_cfgId, info);
            }

        }
    }
    // 获取图集宽高
    public static Vector2 GetTexSize()
    {
        return s_texSize;
    }
    // 根据名字获取图片的位置
    public static FaceFrameCfgInfo GetFaceFrame(string key)
    {
        if (s_allFaceDict == null)
        {
            return null;
        }
        if (s_allFaceDict.ContainsKey(key))
        {
            return s_allFaceDict[key];
        }
        return null;
    }
    // 获取所有表情的key
    public static List<string> GetAllFaceKeyList()
    {
        if (s_allFaceDict == null)
        {
            return null;
        }
        List<string> keyList = new List<string>();
        foreach (string key in s_allFaceDict.Keys)
        {
            keyList.Add(key);
        }
        return keyList;
    }
    // 根据图片获取图片的uv位置
    public static Rect GetFaceUV(string key)
    {
        FaceFrameCfgInfo info = GetFaceFrame(key);
        if (info != null)
        {
            float y = info.m_y / s_texSize.y;
            float h = info.m_h / s_texSize.y;
            float minY = 1 - y - h;
            return new Rect(info.m_x / s_texSize.x, minY, info.m_w / s_texSize.x, h);
        }
        return new Rect(0, 0, 0, 0);
    }
    // 是否存在key
    public static bool IsExistKey(string key)
    {
        GetAllFaceKeyList();
        if (s_allFaceDict == null)
        {
            return false;
        }
        return s_allFaceDict.ContainsKey(key);
    }
}


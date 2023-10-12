using System;
using System.Collections.Generic;
using System.Linq;
using PaddleOCRJson.Enums.StartupArgs;
using PaddleOCRJson.Extensions;

namespace PaddleOCRJson;

public class OcrEngineStartupArgs
{
    internal readonly string EnginePath;
    internal readonly OcrEngineMode EngineMode;
    internal readonly Dictionary<string, object> StartupArgs;


    private OcrEngineStartupArgs(string enginePath, OcrEngineMode engineMode)
    {
        EnginePath = enginePath;
        EngineMode = engineMode;
        StartupArgs = new Dictionary<string, object>();
        // default
    }

    public override string ToString()
    {
        return string.Join(" ", StartupArgs.Select(it => $"-{it.Key}=\"{it.Value}\""));
    }

    #region 工作模式

    public static OcrEngineStartupArgs WithTcpMode(string enginePath, ListenAddresses address, ushort port)
    {
        return new OcrEngineStartupArgs(enginePath, OcrEngineMode.Tcp)
        {
            StartupArgs =
            {
                ["port"] = port,
                ["addr"] = address.RealToString()
            }
        };
    }


    public static OcrEngineStartupArgs WithPipeMode(string enginePath)
    {
        return new OcrEngineStartupArgs(enginePath, OcrEngineMode.Pipe);
    }

    #endregion

    #region 自定义参数

    public OcrEngineStartupArgs Custom(string argName, object value)
    {
        StartupArgs[argName] = value;
        return this;
    }

    #endregion

    #region 常用参数

    public OcrEngineStartupArgs UseGpu(bool value)
    {
        StartupArgs["use_gpu"] = value;
        return this;
    }

    public OcrEngineStartupArgs UseTensorrt(bool value)
    {
        StartupArgs["use_tensorrt"] = value;
        return this;
    }

    public OcrEngineStartupArgs GpuId(int value)
    {
        StartupArgs["gpu_id"] = value;
        return this;
    }

    public OcrEngineStartupArgs GpuMem(int value)
    {
        StartupArgs["gpu_mem"] = value;
        return this;
    }

    public OcrEngineStartupArgs CpuThreads(int value)
    {
        StartupArgs["cpu_threads"] = value;
        return this;
    }

    public OcrEngineStartupArgs EnableMkldnn(bool value)
    {
        StartupArgs["enable_mkldnn"] = value;
        return this;
    }

    public OcrEngineStartupArgs Precision(Precisions value)
    {
        StartupArgs["precision"] = value.RealToString();
        return this;
    }

    public OcrEngineStartupArgs Benchmark(bool value)
    {
        StartupArgs["benchmark"] = value;
        return this;
    }

    public OcrEngineStartupArgs Output(string value)
    {
        StartupArgs["output"] = value;
        return this;
    }

    [Obsolete("Not valid in current ver: 1.3.1")]
    public OcrEngineStartupArgs Type(Types value)
    {
        StartupArgs["type"] = value.RealToString();
        return this;
    }

    public OcrEngineStartupArgs ConfigPath(string value)
    {
        StartupArgs["config_path"] = value;
        return this;
    }

    public OcrEngineStartupArgs EnsureAscii(bool value)
    {
        StartupArgs["ensure_ascii"] = value;
        return this;
    }

    #endregion


    #region DET检测相关

    public OcrEngineStartupArgs DetModelDir(string value)
    {
        StartupArgs["det_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs LimitType(LimitTypes value)
    {
        StartupArgs["limit_type"] = value.RealToString();
        return this;
    }

    public OcrEngineStartupArgs LimitSideLen(int value)
    {
        StartupArgs["limit_side_len"] = value;
        return this;
    }

    public OcrEngineStartupArgs DetDbThresh(double value)
    {
        StartupArgs["det_db_thresh"] = value;
        return this;
    }

    public OcrEngineStartupArgs DetDbBoxThresh(double value)
    {
        StartupArgs["det_db_box_thresh"] = value;
        return this;
    }

    public OcrEngineStartupArgs DetDbUnclipRatio(double value)
    {
        StartupArgs["det_db_unclip_ratio"] = value;
        return this;
    }

    public OcrEngineStartupArgs UseDilation(bool value)
    {
        StartupArgs["use_dilation"] = value;
        return this;
    }

    public OcrEngineStartupArgs DetDbScoreMode(string value)
    {
        StartupArgs["use_dilation"] = value;
        return this;
    }

    public OcrEngineStartupArgs Visualize(bool value)
    {
        StartupArgs["visualize"] = value;
        return this;
    }

    #endregion


    #region CLS方向分类相关

    public OcrEngineStartupArgs UseAngleCls(bool value)
    {
        StartupArgs["use_angle_cls"] = value;
        return this;
    }

    public OcrEngineStartupArgs ClsModelDir(string value)
    {
        StartupArgs["cls_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs ClsThresh(double value)
    {
        StartupArgs["cls_thresh"] = value;
        return this;
    }

    public OcrEngineStartupArgs ClsBatchNum(int value)
    {
        StartupArgs["cls_batch_num"] = value;
        return this;
    }

    #endregion


    #region REC文本识别相关

    public OcrEngineStartupArgs RecModelDir(string value)
    {
        StartupArgs["rec_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs RecBatchNum(int value)
    {
        StartupArgs["rec_batch_num"] = value;
        return this;
    }

    public OcrEngineStartupArgs RecCharDictPath(string value)
    {
        StartupArgs["rec_char_dict_path"] = value;
        return this;
    }

    public OcrEngineStartupArgs RecImgH(int value)
    {
        StartupArgs["rec_img_h"] = value;
        return this;
    }

    public OcrEngineStartupArgs RecImgW(int value)
    {
        StartupArgs["rec_img_w"] = value;
        return this;
    }

    #endregion

    #region 版面分析相关

    public OcrEngineStartupArgs LayoutModelDir(string value)
    {
        StartupArgs["layout_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs LayoutModelPath(string value)
    {
        StartupArgs["layout_dict_path"] = value;
        return this;
    }

    public OcrEngineStartupArgs LayoutScoreThreshold(double value)
    {
        StartupArgs["layout_score_threshold"] = value;
        return this;
    }

    public OcrEngineStartupArgs LayoutNmsThreshold(double value)
    {
        StartupArgs["layout_nms_threshold"] = value;
        return this;
    }

    #endregion

    #region 表格结构相关

    public OcrEngineStartupArgs TableModelDir(string value)
    {
        StartupArgs["table_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs TableMaxLen(int value)
    {
        StartupArgs["table_max_len"] = value;
        return this;
    }

    public OcrEngineStartupArgs TableBatchNum(int value)
    {
        StartupArgs["table_batch_num"] = value;
        return this;
    }

    public OcrEngineStartupArgs MergeNoSpanStructure(bool value)
    {
        StartupArgs["merge_no_span_structure"] = value;
        return this;
    }

    public OcrEngineStartupArgs TableCharDictPath(string value)
    {
        StartupArgs["table_char_dict_path"] = value;
        return this;
    }

    #endregion


    #region 前处理相关

    public OcrEngineStartupArgs Det(bool value)
    {
        StartupArgs["det"] = value;
        return this;
    }

    public OcrEngineStartupArgs Rec(bool value)
    {
        StartupArgs["rec"] = value;
        return this;
    }

    public OcrEngineStartupArgs Cls(bool value)
    {
        StartupArgs["cls"] = value;
        return this;
    }

    public OcrEngineStartupArgs Table(bool value)
    {
        StartupArgs["table"] = value;
        return this;
    }

    public OcrEngineStartupArgs Layout(bool value)
    {
        StartupArgs["layout"] = value;
        return this;
    }

    #endregion
}
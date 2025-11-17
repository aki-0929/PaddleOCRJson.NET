#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

#endregion

namespace PaddleOCRJson;

public class OcrEngineStartupArgs
{
    internal readonly OcrEngineMode EngineMode;
    internal readonly string EnginePath;
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

    #region 自定义参数

    public OcrEngineStartupArgs WithCustom(string argName, object value)
    {
        StartupArgs[argName] = value;
        return this;
    }

    #endregion

    #region 工作模式

    public static OcrEngineStartupArgs WithTcpMode(string enginePath, IPAddress address, ushort port)
    {
        return new OcrEngineStartupArgs(enginePath, OcrEngineMode.Tcp)
        {
            StartupArgs =
            {
                ["port"] = port,
                ["addr"] = address.ToString()
            }
        };
    }


    public static OcrEngineStartupArgs WithPipeMode(string enginePath)
    {
        return new OcrEngineStartupArgs(enginePath, OcrEngineMode.Pipe);
    }

    #endregion

    #region 常用参数

    public OcrEngineStartupArgs WithCpuThreads(int value)
    {
        StartupArgs["cpu_threads"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithCpuMem(int value)
    {
        StartupArgs["cpu_mem"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithEnableMkldnn(bool value)
    {
        StartupArgs["enable_mkldnn"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithPrecision(string value)
    {
        StartupArgs["precision"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithBenchmark(bool value)
    {
        StartupArgs["benchmark"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithOutput(string value)
    {
        StartupArgs["output"] = value;
        return this;
    }

    [Obsolete("Not valid in current ver: 1.3.1")]
    public OcrEngineStartupArgs WithType(string value)
    {
        StartupArgs["type"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithConfigPath(string value)
    {
        StartupArgs["config_path"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithModelsPath(string value)
    {
        StartupArgs["models_path"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithEnsureAscii(bool value)
    {
        StartupArgs["ensure_ascii"] = value;
        return this;
    }

    #endregion

    #region DET检测相关

    public OcrEngineStartupArgs WithDetModelDir(string value)
    {
        StartupArgs["det_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithLimitType(string value)
    {
        StartupArgs["limit_type"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithLimitSideLen(int value)
    {
        StartupArgs["limit_side_len"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithDetDbThresh(double value)
    {
        StartupArgs["det_db_thresh"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithDetDbBoxThresh(double value)
    {
        StartupArgs["det_db_box_thresh"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithDetDbUnclipRatio(double value)
    {
        StartupArgs["det_db_unclip_ratio"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithUseDilation(bool value)
    {
        StartupArgs["use_dilation"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithDetDbScoreMode(string value)
    {
        StartupArgs["det_db_score_mode"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithVisualize(bool value)
    {
        StartupArgs["visualize"] = value;
        return this;
    }

    #endregion

    #region CLS方向分类相关

    public OcrEngineStartupArgs WithUseAngleCls(bool value)
    {
        StartupArgs["use_angle_cls"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithClsModelDir(string value)
    {
        StartupArgs["cls_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithClsThresh(double value)
    {
        StartupArgs["cls_thresh"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithClsBatchNum(int value)
    {
        StartupArgs["cls_batch_num"] = value;
        return this;
    }

    #endregion

    #region REC文本识别相关

    public OcrEngineStartupArgs WithRecModelDir(string value)
    {
        StartupArgs["rec_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithRecBatchNum(int value)
    {
        StartupArgs["rec_batch_num"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithRecCharDictPath(string value)
    {
        StartupArgs["rec_char_dict_path"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithRecImgH(int value)
    {
        StartupArgs["rec_img_h"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithRecImgW(int value)
    {
        StartupArgs["rec_img_w"] = value;
        return this;
    }

    #endregion

    #region 版面分析相关

    public OcrEngineStartupArgs WithLayoutModelDir(string value)
    {
        StartupArgs["layout_model_dir"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithLayoutDictPath(string value)
    {
        StartupArgs["layout_dict_path"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithLayoutScoreThreshold(double value)
    {
        StartupArgs["layout_score_threshold"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithLayoutNmsThreshold(double value)
    {
        StartupArgs["layout_nms_threshold"] = value;
        return this;
    }

    #endregion

    #region 前处理相关

    public OcrEngineStartupArgs WithDet(bool value)
    {
        StartupArgs["det"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithRec(bool value)
    {
        StartupArgs["rec"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithCls(bool value)
    {
        StartupArgs["cls"] = value;
        return this;
    }

    public OcrEngineStartupArgs WithLayout(bool value)
    {
        StartupArgs["layout"] = value;
        return this;
    }

    #endregion
}
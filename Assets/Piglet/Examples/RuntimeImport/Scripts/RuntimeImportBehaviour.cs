using UnityEngine;
using System;
using Piglet;

public class RuntimeImportBehaviour : MonoBehaviour
{
    /// <summary>
    /// The currently running glTF import task.
    /// </summary>
    private GltfImportTask _task;

    /// <summary>
    /// Root GameObject of the imported glTF model.
    /// </summary>
    private GameObject _model;

    /// <summary>
    /// Callback khi import thành công.
    /// </summary>
    private Action<GameObject> _onSuccess;

    /// <summary>
    /// Callback khi có lỗi.
    /// </summary>
    private Action<string> _onError;

    /// <summary>
    /// Bắt đầu quá trình import mô hình GLB.
    /// </summary>
    /// <param name="glbFilePath">Đường dẫn tới tệp GLB.</param>
    /// <param name="onSuccess">Callback khi thành công.</param>
    /// <param name="onError">Callback khi có lỗi.</param>
    public void ImportModel(string glbFilePath, Action<GameObject> onSuccess, Action<string> onError)
    {
        _onSuccess = onSuccess;
        _onError = onError;

        _task = RuntimeGltfImporter.GetImportTask(glbFilePath);
        _task.OnProgress = OnProgress;
        _task.OnCompleted = OnComplete;
    }

    /// <summary>
    /// Callback khi import hoàn thành.
    /// </summary>
    /// <param name="importedModel">GameObject của mô hình đã import.</param>
    private void OnComplete(GameObject importedModel)
    {
        _model = importedModel;
        Debug.Log("Mô hình 3D đã được tải thành công.");
        _onSuccess?.Invoke(_model);
    }

    /// <summary>
    /// Callback để báo cáo tiến trình import.
    /// </summary>
    private void OnProgress(GltfImportStep step, int completed, int total)
    {
        Debug.LogFormat("{0}: {1}/{2}", step, completed, total);
    }

    /// <summary>
    /// Unity callback được gọi mỗi frame.
    /// Tiến hành import mô hình từng bước.
    /// </summary>
    void Update()
    {
        if (_task != null)
        {
            try
            {
                _task.MoveNext();
            }
            catch (Exception ex)
            {
                _onError?.Invoke("Lỗi khi import mô hình: " + ex.Message);
                _task = null;
            }
        }
    }
}

using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Classe auxiliar para lidar com tarefas assíncronas em corrotinas de forma segura
/// </summary>
public static class AsyncTaskHandler
{
    /// <summary>
    /// Executa uma task assíncrona em uma corrotina, lidando com erros de forma segura
    /// </summary>
    public static IEnumerator HandleTask<T>(Task<T> task, System.Action<T> onSuccess, System.Action<System.Exception> onError = null)
    {
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.Exception != null)
        {
            var baseException = task.Exception.GetBaseException();
            Debug.LogError($"[AsyncTaskHandler] Erro na task: {baseException.Message}");
            onError?.Invoke(baseException);
        }
        else if (task.IsCanceled)
        {
            Debug.LogWarning("[AsyncTaskHandler] Task foi cancelada");
            onError?.Invoke(new TaskCanceledException("Task foi cancelada"));
        }
        else
        {
            onSuccess?.Invoke(task.Result);
        }
    }
    
    /// <summary>
    /// Versão para tasks sem retorno
    /// </summary>
    public static IEnumerator HandleTask(Task task, System.Action onSuccess, System.Action<System.Exception> onError = null)
    {
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.Exception != null)
        {
            var baseException = task.Exception.GetBaseException();
            Debug.LogError($"[AsyncTaskHandler] Erro na task: {baseException.Message}");
            onError?.Invoke(baseException);
        }
        else if (task.IsCanceled)
        {
            Debug.LogWarning("[AsyncTaskHandler] Task foi cancelada");
            onError?.Invoke(new TaskCanceledException("Task foi cancelada"));
        }
        else
        {
            onSuccess?.Invoke();
        }
    }
}
// TODO: Create a flow explanation to explain how this decompiled code works
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.0.0.0")]
[module: UnverifiableCode]
[module: RefSafetyRules(11)]

[NullableContext(1)]
[Nullable(0)]
internal class LibraryService
{
    [StructLayout(LayoutKind.Auto)]
    [CompilerGenerated]
    private struct <GetLibraries>d__2 : IAsyncStateMachine
    {
        public int <>1__state;

        [Nullable(new byte[] { 0, 0, 1 })]
        public AsyncTaskMethodBuilder<List<LibraryModel>> <>t__builder;

        [Nullable(0)]
        public LibraryService <>4__this;

        [Nullable(new byte[] { 0, 1 })]
        private TaskAwaiter<HttpResponseMessage> <>u__1;

        [Nullable(new byte[] { 0, 1 })]
        private TaskAwaiter<Stream> <>u__2;

        [Nullable(new byte[] { 0, 2, 1 })]
        private ValueTaskAwaiter<List<LibraryModel>> <>u__3;

        private void MoveNext()
        {
            int num = <>1__state;
            LibraryService libraryService = <>4__this;
            List<LibraryModel> result3;
            try
            {
                TaskAwaiter<HttpResponseMessage> awaiter3;
                TaskAwaiter<Stream> awaiter2;
                ValueTaskAwaiter<List<LibraryModel>> awaiter;
                HttpResponseMessage result;
                switch (num)
                {
                    default:
                        awaiter3 = libraryService.<httpClient>P.GetAsync("some domain").GetAwaiter();
                        if (!awaiter3.IsCompleted)
                        {
                            num = (<>1__state = 0);
                            <>u__1 = awaiter3;
                            <>t__builder.AwaitUnsafeOnCompleted(ref awaiter3, ref this);
                            return;
                        }
                        goto IL_007e;
                    case 0:
                        awaiter3 = <>u__1;
                        <>u__1 = default(TaskAwaiter<HttpResponseMessage>);
                        num = (<>1__state = -1);
                        goto IL_007e;
                    case 1:
                        awaiter2 = <>u__2;
                        <>u__2 = default(TaskAwaiter<Stream>);
                        num = (<>1__state = -1);
                        goto IL_00e7;
                    case 2:
                        {
                            awaiter = <>u__3;
                            <>u__3 = default(ValueTaskAwaiter<List<LibraryModel>>);
                            num = (<>1__state = -1);
                            break;
                        }
                        IL_00e7:
                        awaiter = JsonSerializer.DeserializeAsync<List<LibraryModel>>(awaiter2.GetResult()).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            num = (<>1__state = 2);
                            <>u__3 = awaiter;
                            <>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                            return;
                        }
                        break;
                        IL_007e:
                        result = awaiter3.GetResult();
                        result.EnsureSuccessStatusCode();
                        awaiter2 = result.Content.ReadAsStreamAsync().GetAwaiter();
                        if (!awaiter2.IsCompleted)
                        {
                            num = (<>1__state = 1);
                            <>u__2 = awaiter2;
                            <>t__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
                            return;
                        }
                        goto IL_00e7;
                }
                List<LibraryModel> result2 = awaiter.GetResult();
                if (result2 == null)
                {
                    throw new InvalidOperationException("Lib");
                }
                result3 = result2;
            }
            catch (Exception exception)
            {
                <>1__state = -2;
                <>t__builder.SetException(exception);
                return;
            }
            <>1__state = -2;
            <>t__builder.SetResult(result3);
        }

        void IAsyncStateMachine.MoveNext()
        {
            //ILSpy generated this explicit interface implementation from .override directive in MoveNext
            this.MoveNext();
        }

        [DebuggerHidden]
        private void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            <>t__builder.SetStateMachine(stateMachine);
        }

        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            //ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
            this.SetStateMachine(stateMachine);
        }
    }

    [CompilerGenerated]
    private HttpClient <httpClient>P;

    public LibraryService(HttpClient httpClient)
    {
        <httpClient>P = httpClient;
        base..ctor();
    }

    [AsyncStateMachine(typeof(<GetLibraries>d__2))]
    public Task<List<LibraryModel>> GetLibraries()
    {
        <GetLibraries>d__2 stateMachine = default(<GetLibraries>d__2);
        stateMachine.<>t__builder = AsyncTaskMethodBuilder<List<LibraryModel>>.Create();
        stateMachine.<>4__this = this;
        stateMachine.<>1__state = -1;
        stateMachine.<>t__builder.Start(ref stateMachine);
        return stateMachine.<>t__builder.Task;
    }
}

[NullableContext(1)]
[Nullable(0)]
[RequiredMember]
internal class LibraryModel
{
    [RequiredMember]
    public string something;

    [RequiredMember]
    public string something2;

    [RequiredMember]
    public string something3;

    [Obsolete("Constructors of types with required members are not supported in this version of your compiler.", true)]
    [CompilerFeatureRequired("RequiredMembers")]
    public LibraryModel()
    {
    }
}
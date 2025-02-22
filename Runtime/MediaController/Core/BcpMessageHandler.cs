// Visual Pinball Engine
// Copyright (C) 2025 freezy and VPE Team
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Monitor;

namespace VisualPinball.Engine.Mpf.Unity.MediaController
{
    public interface IBcpMessageHandler
    {
        public void Handle(BcpMessage message);
    }

    public abstract class BcpMessageHandler<T> : IBcpMessageHandler
        where T : EventArgs
    {
        public abstract string Command { get; }
        public virtual MonitoringCategory MonitoringCategory => MonitoringCategory.None;
        protected abstract ParseDelegate Parse { get; }
        public delegate T ParseDelegate(BcpMessage genericMessage);
        private event EventHandler<T> _received;
        public event EventHandler<T> Received
        {
            add
            {
                bool isFirstListener = _received == null;
                _received += value;
                if (isFirstListener && MonitoringCategory != MonitoringCategory.None)
                    _bcpInterface.MonitoringCategories.AddListener(this, MonitoringCategory);
            }
            remove
            {
                _received -= value;
                bool noMoreListeners = _received == null;
                if (noMoreListeners && MonitoringCategory != MonitoringCategory.None)
                    _bcpInterface.MonitoringCategories.RemoveListener(this, MonitoringCategory);
            }
        }

        private BcpInterface _bcpInterface;

        public BcpMessageHandler(BcpInterface bcpInterface)
        {
            _bcpInterface = bcpInterface;
        }

        public void Handle(BcpMessage message)
        {
            if (message.Command != Command)
                throw new WrongParserException(message, Command);
            T specificMessage = Parse(message);
            BeforeReceivedEvent();
            _received?.Invoke(this, specificMessage);
            AfterReceivedEvent();
        }

        protected virtual void BeforeReceivedEvent() { }

        protected virtual void AfterReceivedEvent() { }
    }
}

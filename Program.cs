using RabbitManagement;
using RabbitMQ.Client.Events;
using static System.Console;

namespace RabbitMqSample
{
    class Program
    {

        static QueuManager _queuManager;

        static void Main(string[] args)
        {
            _queuManager = new QueuManager("admin", "admin");

            _queuManager.ReceiveMessage += _queuManager_ReceiveMessage;


            //WORKING WITH FANOUT EXCHANGE
            _queuManager.CreateExchangeFanout("all-queue", true, _queuManager.Connection);
            _queuManager.CreateQueue("Fila1", _queuManager.Connection);
            _queuManager.BindingQueue("Fila1", "all-queue", _queuManager.Connection);
            _queuManager.Enqueue("Teste 2", _queuManager.Connection, "all-queue");

            //WORKIN WITH TOPIC EXCHANGE
            _queuManager.CreateExchangeTopic("only-topi-queue", true, _queuManager.Connection);
            _queuManager.CreateQueue("FilaToday", _queuManager.Connection);
            _queuManager.BindingQueue("FilaToday", "only-topi-queue", _queuManager.Connection, "*.user");
            _queuManager.Enqueue("Topic message", _queuManager.Connection, "only-topi-queue", "reset.user");


            //WORKIN WITH DIRECT EXCHANGE

            string exchangeDirect = "directexchange";

            string nomeFilaDirect1 = "fila3";
            string nomeFilaDirect2 = "fila4";

            _queuManager.CreateExchangeDirect(exchangeDirect, true, _queuManager.Connection);
            _queuManager.CreateQueue(nomeFilaDirect1, _queuManager.Connection);
            _queuManager.CreateQueue(nomeFilaDirect2, _queuManager.Connection);

            _queuManager.BindingQueue(nomeFilaDirect1, exchangeDirect, _queuManager.Connection, "Teste1");
            _queuManager.BindingQueue(nomeFilaDirect2, exchangeDirect, _queuManager.Connection, "Teste2");
            _queuManager.BindingQueue(nomeFilaDirect2, exchangeDirect, _queuManager.Connection, "*.Teste"); /*Funciona apenas com Topic*/

            _queuManager.Enqueue("Apenas para a fila 3", _queuManager.Connection, exchangeDirect, "Teste1");
            _queuManager.Enqueue("Apenas para a fila 4", _queuManager.Connection, exchangeDirect, "Teste2");
            _queuManager.Enqueue("Para a fila 3 e 4", _queuManager.Connection, exchangeDirect, "Terceiro.Teste"); /*Funciona apenas com Topic*/


            WriteLine("Mensagens enfileiradas");

            ReadKey();
        }

        private static void _queuManager_ReceiveMessage(object sender, BasicDeliverEventArgs e)
        {
            WriteLine($"MEnsagem recebida: {e.Body}");
        }
    }
}

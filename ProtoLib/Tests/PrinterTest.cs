using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProtoLib.Tests;

[TestFixture]
public class PrinterTest
{

    [Test]

    public void TestPDFTransfer()
    {
        TransferPrinter tp = new TransferPrinter(Environment.CurrentDirectory);
        using (BaseContext c = new BaseContext(""))
        {
            tp.MakeTransferPdf(c.Transfers.Include(x => x.Lines).First());
        }
        

    }
}
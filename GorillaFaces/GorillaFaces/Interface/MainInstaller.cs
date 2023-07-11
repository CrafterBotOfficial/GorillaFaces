using ComputerInterface.Interfaces;
using Zenject;

namespace GorillaFaces.Interface
{
    internal class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IComputerModEntry>().To<Views.MainView.Entry>().AsSingle();
        }
    }
}

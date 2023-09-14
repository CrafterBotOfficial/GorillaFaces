using ComputerInterface.Interfaces;
using Zenject;

namespace GorillaFaces.Interface
{
    public class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IComputerModEntry>().To<Views.MainView.Entry>().AsSingle();
        }
    }
}

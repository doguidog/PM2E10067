using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.Media;
using PM2E10067.Vista;

namespace PM2E10067
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Clear()
        {
            txtLat.Text = "";
            txtLon.Text = "";
            txtDescripcion.Text = "";
        } 

        public async void LoadCoord()
        {
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                if (location != null)
                {

                    var lon = location.Longitude;
                    var lat = location.Latitude;

                    txtLat.Text = lat.ToString();
                    txtLon.Text = lon.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS" + fnsEx, "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Advertencia", "Error de Dispositivo, validar si su GPS esta activo", "Ok");
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Advertencia", "Sin Permisos de Geolocalizacion" + pEx, "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicacion " + ex, "Ok");
            }
        }

        Plugin.Media.Abstractions.MediaFile Filefoto = null;

        private Byte[] ConvertImageToByteArray()
        {
            if (Filefoto != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = Filefoto.GetStream();
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }

            }
            return null;

        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            Filefoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "MisFotos",
                Name = "test.jpg",
                SaveToAlbum = true,
            });


            if (Filefoto != null)
            {
                fotoSitio.Source = ImageSource.FromStream(() =>
                {
                    return Filefoto.GetStream();
                });
            }
        }

       private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            if (Filefoto == null)
            {
                await this.DisplayAlert("Advertencia", "Debe tomar una foto", "OK");
            }
            else if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                await this.DisplayAlert("Advertencia", "El campo del Descripcion es obligatorio.", "OK");

            }
            else if (string.IsNullOrEmpty(txtLat.Text) && string.IsNullOrEmpty(txtLon.Text))
            {
                await this.DisplayAlert("Advertencia", "No se puede agregar Registro. Faltan coordenadas.", "OK");

                LoadCoord();

            }
            else
            {
                var sitio = new Modelo.Sitios
                {
                    id = 0,
                    latitud = txtLat.Text,
                    longitud = txtLon.Text,
                    descripcion = txtDescripcion.Text,
                    foto = ConvertImageToByteArray(),
                };

                // await DisplayAlert("Aviso", "Sitio Adicionado" + sitio.foto, "OK");
                var result = await App.DBase.SitioSave(sitio);

                if (result > 0)//se usa como una super clase
                {
                    await DisplayAlert("Aviso", "Sitio Registrado", "OK");
                    Clear();

                }
                else
                {
                    await DisplayAlert("Aviso", "Error al Registrar", "OK");
                }
            }
        }


        private void btnSalir_Clicked(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            LoadCoord();
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                if (location != null)
                {

                    var lon = location.Longitude;
                    var lat = location.Latitude;

                    txtLat.Text = lat.ToString();
                    txtLon.Text = lon.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Advertencia", "Este dispositivo no soporta GPS" + fnsEx, "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Advertencia", "Error de Dispositivo, validar si su GPS esta activo", "Ok");
                System.Diagnostics.Process.GetCurrentProcess().Kill(); //cerramos la aplicacion hasta que el usuario active el GPS

            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Advertencia", "Sin Permisos de Geolocalizacion" + pEx, "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Advertencia", "Sin Ubicacion " + ex, "Ok");
            }
        }

        private async void btnList_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListLugares());
        }
    }
}

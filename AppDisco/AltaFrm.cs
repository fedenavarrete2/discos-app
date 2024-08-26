using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;
using System.Configuration;

namespace AppDisco
{
    public partial class AltaFrm : Form
    {


        private Disco disco = null;
        private OpenFileDialog archivo = null;
        public AltaFrm()
        {
            InitializeComponent();
            Text = "Agregar disco";
        }

        public AltaFrm(Disco disco)
        {
            InitializeComponent();
            this.disco = disco;
            Text = "Modificar Disco";
        }

        private bool validarDatos()
        {
            bool validar = true;
            if(txtNombre.Text == string.Empty)
            {
                MessageBox.Show("Debe cargar un nombre.");
                validar = false;
            }
            /*else if(txtCantidad.Text == string.Empty)
            {
                MessageBox.Show("Debe cargar la cantidad de canciones.");
                validar = false;
            }*/
            else if(cboEstilo.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un estilo.");
                validar = false;
            }
            else if(cboEdicion.SelectedIndex ==-1)
            {
                MessageBox.Show("Debe seleccionar un tipo de edición.");
                validar = false;
            }       
                    return validar;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            DiscoNegocio negocio = new DiscoNegocio();
            validarDatos();
            try
            {
                if (disco == null)
                disco = new Disco();
                disco.Nombre = txtNombre.Text;
                disco.FechaLanzamiento = Convert.ToDateTime(dtpFecha.Value.ToString("dd/MM/yyyy"));
                disco.CantCanciones = Convert.ToInt32(txtCantidad.Text);
                disco.UrlImagen = txtUrlImagen.Text;
                disco.Tipo = (Estilo)cboEstilo.SelectedItem;
                disco.Edicion = (TipoEdicion)cboEdicion.SelectedItem;


                if (disco.Codigo != 0)
                {
                    negocio.modificar(disco);
                    MessageBox.Show("Modificado correctamente.");
                }
                else
                {
                    negocio.agregar(disco);
                    MessageBox.Show("Agregado exitosamente");                  
                }

               
            }
            catch(FormatException ex)
            {
                {
                    MessageBox.Show("Debe cargar la cantidad de canciones.");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
            {
               File.Copy(archivo.FileName, ConfigurationManager.AppSettings["image-folder"] + archivo.SafeFileName);
            }

            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AltaFrm_Load(object sender, EventArgs e)
        {

            EstiloNegocio estiloNegocio = new EstiloNegocio();
            TipoEdicionNegocio edicionNegocio = new TipoEdicionNegocio();

            try
            {
                cboEstilo.DataSource = estiloNegocio.listar();
                cboEstilo.ValueMember = "codigo";
                cboEstilo.DisplayMember = "descripcion";
                cboEstilo.DropDownStyle = ComboBoxStyle.DropDownList;


                cboEdicion.DataSource = edicionNegocio.listar();
                cboEdicion.DropDownStyle = ComboBoxStyle.DropDownList;
                cboEdicion.ValueMember = "id";
                cboEdicion.DisplayMember = "descripcion";

                if (disco != null)
                {
                    txtNombre.Text = disco.Nombre;
                    dtpFecha.Value = disco.FechaLanzamiento;
                    txtCantidad.Text = disco.CantCanciones.ToString();
                    txtUrlImagen.Text = disco.UrlImagen;
                    cargarImagen(disco.UrlImagen);
                    cboEstilo.SelectedValue = disco.Tipo.Codigo;
                   cboEdicion.SelectedValue = disco.Edicion.Id;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxDiscos.Load(imagen);
            }
            catch (Exception ex)
            {

                pbxDiscos.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void txtUrlImagen_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }

        private void btnImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);              
            }        
        }
    }
}

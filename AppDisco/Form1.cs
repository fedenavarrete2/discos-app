using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace AppDisco
{
    public partial class Discos : Form
    {
        private List<Disco> listaDisco;
        public Discos()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Seguro que desea salir?", "SALIR", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes )
             this.Close() ;
        }

        private void Discos_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Cant.Canciones");
        }

        private void cargar()
        {
            DiscoNegocio negocio = new DiscoNegocio();
            listaDisco = negocio.listar();
            dgvDiscos.DataSource = listaDisco;
            cargarImagen(listaDisco[0].UrlImagen);
            ocultarColumnas();
        }

        private void ocultarColumnas()
        {
            dgvDiscos.Columns["UrlImagen"].Visible = false;
            dgvDiscos.Columns[0].Visible = false;
        }


        private void dgvDiscos_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                Disco seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
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

                pbxDiscos.Load(listaDisco[0].UrlImagen);
            }   
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AltaFrm alta = new AltaFrm();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Disco seleccionado = new Disco();
            seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;
            AltaFrm modificar = new AltaFrm(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            DiscoNegocio negocio = new DiscoNegocio();
            Disco eliminado;

            try
            {
                if(MessageBox.Show("Seguro que desea borrar este disco?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) 
                    == DialogResult.Yes)
                {
                    eliminado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;
                    negocio.eliminar(eliminado.Codigo);
                    cargar();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Disco> listaFiltrada;
            string buscar = txtFiltro.Text;
            try
            {
                if (buscar.Length >= 3)
                {
                    listaFiltrada = listaDisco.FindAll(x => x.Nombre.ToUpper().Contains(buscar.ToUpper()));
                }
                else
                {
                    listaFiltrada = listaDisco;
                }
                
                dgvDiscos.DataSource = listaFiltrada;
                ocultarColumnas();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }
            return true;
        }


        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if(opcion == "Nombre")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con:");
                cboCriterio.Items.Add("Termina con:");
                cboCriterio.Items.Add("Contiene:");
            }
            else
            {

                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a:");
                cboCriterio.Items.Add("Menor a:");
                cboCriterio.Items.Add("Igual a:");
            }
        }

        private bool validarFiltro()
        {
        if(cboCampo.SelectedIndex < 0)
        {
                MessageBox.Show("Debe seleccionar un campo");
                return true;
        }
        else if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar un criterio.");
                return true;
            }
        else if(cboCampo.SelectedIndex == 1)
            {
                if(txtBuscar.Text == string.Empty)
                {
                    MessageBox.Show("Debe escribir un número.");
                    return true;
                }
                else if(!(soloNumeros(txtBuscar.Text)))
                {
                    MessageBox.Show("Solo se puede escribir números.");
                    return true;
                 }
            }

            return false;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            DiscoNegocio negocio = new DiscoNegocio();
            try
            {
                if (validarFiltro())
                    return;
                    string campo = cboCampo.SelectedItem.ToString();
                    string criterio = cboCriterio.SelectedItem.ToString();
                    string filtro = txtBuscar.Text;
                    dgvDiscos.DataSource = negocio.filtrar(campo, criterio, filtro);
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
    }
}

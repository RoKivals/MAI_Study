// nvcc main.cu -lGL -lGLU -lglut -lGLEW
// ./a.out

// Для linux нужно поставить пакеты: libgl1-mesa-dev libglew-dev freeglut3-dev
// sudo apt-get install libgl1-mesa-dev libglew-dev freeglut3-dev


#define CSC(call) 				\
do {						\
	cudaError_t status = call;		\
	if (status != cudaSuccess) {		\
		fprintf(stderr, "ERROR is %s:%d. Message: %s\n", __FILE__, __LINE__, cudaGetErrorString(status));	\
		exit(0);			\
	}					\
} while(0)


#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <GL/glew.h>
#include <GL/freeglut.h>
#include <cuda_runtime.h>
#include <cuda_gl_interop.h>

const int w = 1024;
const int h = 748;

const double xc = 0.0, yc = 0.0, sx = 5.0, sy = sx * h / w, minf = -3.0, maxf = 3.0;

__device__ double fun(double x, double y, double t) {
	return sin(x * x + t) + cos(y * y + 0.6 * t) + sin(x * x + y * y + 0.3 * t);
}

__device__ double fun(int i, int j, double t) {
	double x = 2.0 * i / (double)(w - 1) - 1.0;  	// i / (double)(w - 1) -> [0, 1];   2.0 * i / (double)(w - 1) -> [0, 2];   [-1, 1]
	double y = 2.0 * j / (double)(h - 1) - 1.0;
	return fun(xc + x * sx, yc + y * sy, t);		// [-1, 1] * 5.0 -> [-5, 5] + 0 -> [-5, 5] 
}


struct cudaGraphicsResource *res;
GLuint vbo;


__global__ void kernel(uchar4 *data, double t) {
	int idx = blockDim.x * blockIdx.x + threadIdx.x;
	int idy = blockDim.y * blockIdx.y + threadIdx.y;
	int offsetx = gridDim.x * blockDim.x;
	int offsety = gridDim.y * blockDim.y;
	int i, j;
	for(i = idx; i < w; i += offsetx)
		for(j = idy; j < h; j += offsety) {
			double f = (fun(i, j, t) - minf) / (maxf - minf); // [0, 1]
			data[j * w + i] = make_uchar4(0, (int)(f * 255), 0, 255);
		}
}


void update() {
	static double t = 0.0;
	uchar4 *data;
	size_t size;
	cudaGraphicsMapResources(1, &res, 0);
	cudaGraphicsResourceGetMappedPointer((void **)&data, &size, res);
	kernel<<<dim3(32, 32), dim3(16, 16)>>>(data, t);
	cudaGraphicsUnmapResources(1, &res, 0);
	glutPostRedisplay();
	t += 0.05;
}


void display() {
	glClearColor(0.0, 0.0, 0.0, 0.0);
	glClear(GL_COLOR_BUFFER_BIT);
	glDrawPixels(w, h, GL_RGBA, GL_UNSIGNED_BYTE, 0);
	glutSwapBuffers();
}


void keys(unsigned char key, int x, int y) {
	if (key == 27) {
		cudaGraphicsUnregisterResource(res);
		glBindBuffer(1, vbo);
		glDeleteBuffers(1, &vbo);
		exit(0);
	}
}


int main(int argc, char **argv) {
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGBA);
	glutInitWindowSize(w, h);
	glutCreateWindow("Hot Map");

	glutIdleFunc(update);
	glutDisplayFunc(display);
	glutKeyboardFunc(keys);
	
	glMatrixMode(GL_PROJECTION);
		glLoadIdentity();
		gluOrtho2D(0.0, (GLdouble)w, 0.0, (GLdouble)h);
		
	glewInit();
	glGenBuffers(1, &vbo);
	glBindBuffer(GL_PIXEL_UNPACK_BUFFER_ARB, vbo);
	glBufferData(GL_PIXEL_UNPACK_BUFFER_ARB, w * h * sizeof(uchar4), NULL, GL_DYNAMIC_DRAW);
	CSC(cudaGraphicsGLRegisterBuffer(&res, vbo, cudaGraphicsMapFlagsWriteDiscard));

	glutMainLoop();
	return 0;
}

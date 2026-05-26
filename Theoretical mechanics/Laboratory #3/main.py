import numpy as np
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
from scipy.integrate import odeint
import sympy as sp

def formY(y, t, fV, fOm):
    y1,y2,y3,y4 = y   
    dydt = [y3,y4,fV(y1,y2,y3,y4),fOm(y1,y2,y3,y4)]
    return dydt

Length = 4
P = 0.5
l = 2
c = 40
g = 9.81
mu = 1
t = sp.Symbol('t')
phi = sp.Function('phi')(t)
s = sp.Function('s')(t)
om1 = sp.diff(phi,t)
om2 = sp.diff(s,t)

stickX=l*sp.cos(phi)
stickY=l*sp.sin(phi)
springX=stickX+ s*sp.cos(phi)
springY=stickY+ s*sp.sin(phi)
VxSpr=sp.diff(springX,t)
VySpr= sp.diff(springY,t)
VSpr = sp.sqrt(VxSpr**2+VySpr**2)
WxSpr=sp.diff(VxSpr,t)
WySpr= sp.diff(VySpr,t)
WSpr = sp.sqrt(WxSpr**2+WySpr**2)
delta = sp.sqrt((springX-stickX)**2+(springY-stickY)**2)
Epot=P*g*(l+s)*springY+1/2*c*(delta**2)
Ekin=P*(l+s)**2*(sp.diff(phi,t)**2)/2+(1/2)*P*sp.diff(s,t)**2
L=Ekin-Epot

ur1 = sp.diff(sp.diff(L, om1), t) - sp.diff(L, phi).simplify()
ur2 = sp.diff(sp.diff(L, om2), t) - sp.diff(L, s).simplify()
a11 = ur1.coeff(sp.diff(om1, t),1)
a12 = ur1.coeff(sp.diff(om2, t),1)
a21 = ur2.coeff(sp.diff(om1, t),1)
a22 = ur2.coeff(sp.diff(om2, t),1)
b1 = -(ur1.coeff(sp.diff(om1, t),0)).coeff(sp.diff(om2, t),0).subs([(sp.diff(phi, t),om1), (sp.diff(s ,t), om2)])
b2 = -(ur2.coeff(sp.diff(om1, t),0)).coeff(sp.diff(om2, t),0).subs([(sp.diff(phi, t),om1), (sp.diff(s ,t), om2)])

detA = (a11*a22 - a12*a21).simplify()
detA1 = b1*a22 - b2*a12
detA2 = a11*b2 - b1*a21

dVdt = detA1/detA
domdt = detA2/detA

countOfFrames = 400
T_start, T_stop = 0, 10
T = np.linspace(T_start, T_stop, countOfFrames)

fom1 = sp.lambdify([phi,s, om1, om2], dVdt, "numpy")
fom2 = sp.lambdify([phi,s, om1, om2], domdt, "numpy")
y0 = [0, 0, 0, 0]
sol = odeint(formY, y0, T, args = (fom1, fom2))

StickX=l*np.cos(sol[:,0])
StickY=l*np.sin(sol[:,0])
SpringX=StickX+ sol[:,1]*np.cos(sol[:,0])
SpringY=StickY+ sol[:,1]*np.sin(sol[:,0])
StickX=StickX*(l+max(sol[:,1]))/l
StickY=StickY*(l+max(sol[:,1]))/l
VSpr=sol[:,2]
WSpr=sol[:,3]

SuspensionX = -1
SuspensionY =  1
spSteps = 20
spWidth = 0.05

bracingSize = 0.1
bracingX = [SuspensionX, SuspensionX - bracingSize , SuspensionX + bracingSize, SuspensionX]
bracingY = [SuspensionY, SuspensionY + 2*bracingSize , SuspensionY + 2*bracingSize, SuspensionY]

fig = plt.figure(figsize=(17, 8))
ax1 = fig.add_subplot(1, 2, 1)
ax1.set(xlim=[SuspensionX-1.1*Length,SuspensionX + 1.1*Length],ylim=[-2*Length, max(bracingY)*1.01])
ax1.set_xlabel('Axis x')
ax1.set_ylabel('Axis y')

PHI = fig.add_subplot(2, 2, 2)
PHI.set_xlabel('T')
PHI.set_ylabel('V')
PHI.set(xlim=[T_start, T_stop], ylim=[VSpr.min(), VSpr.max()])
tv_x = [T[0]]
tv_y = [VSpr[0]]
TV, = PHI.plot(tv_x, tv_y, '-')

S   = fig.add_subplot(2, 2, 4)
S.set_xlabel('T')
S.set_ylabel('W')
S.set(xlim=[T_start, T_stop], ylim=[WSpr.min(), WSpr.max()])
tw_x = [T[0]]
tw_y = [WSpr[0]]
TW, = S.plot(tw_x, tw_y, '-')


Stick, = ax1.plot([SuspensionX, StickX[0] + SuspensionX], [SuspensionY, StickY[0] + SuspensionY],color='r')
Body,  = ax1.plot( [SpringX[0] + SuspensionX], [ SpringY[0] + SuspensionY],marker='o', color="black", markersize=8)
Spring, = ax1.plot([SuspensionX, SpringX[0] + SuspensionX], [SuspensionY, SpringY[0] + SuspensionY], color="black",linestyle=':', linewidth=3)
bracing, = ax1.plot(bracingX, bracingY, color="black")

def anima(i):
    Stick.set_data([SuspensionX, StickX[i] + SuspensionX], [SuspensionY, StickY[i] + SuspensionY])
    Body.set_data([SpringX[i] + SuspensionX], [ SpringY[i] + SuspensionY])
    Spring.set_data([SuspensionX, SpringX[i] + SuspensionX], [SuspensionY, SpringY[i] + SuspensionY])
    tv_x.append(T[i])
    tv_y.append(VSpr[i])
    tw_x.append(T[i])
    tw_y.append(WSpr[i])
    TV.set_data(tv_x, tv_y)
    TW.set_data(tw_x, tw_y)
    if i == countOfFrames-1:
        tv_x.clear()
        tv_y.clear()
        tw_x.clear()
        tw_y.clear()
    return Stick, Body, Spring, TV, TW

anim = FuncAnimation(fig, anima, frames=countOfFrames, interval=100, blit=True)
plt.show()
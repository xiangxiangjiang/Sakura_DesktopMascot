import requests
import os
import zipfile
import json
import time
from urllib.request import urlretrieve
from sys import argv
from bs4 import BeautifulSoup


def get_html(url):
    try:
        headers = {"Host": "github.com",
                   "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36"}
        r = requests.get(url, timeout=15, headers=headers)
        r.raise_for_status()
        # print(r.status_code)
        # 这里我们知道编码是utf-8，爬其他的页面时建议使用：
        # r.endcodding = r.apparent_endconding
        r.encoding = 'utf-8'
        return r.text
    except:
        return None


def get_GithubData(url):

    posts = {}
    html = get_html(url)
    if html == None:
        print('网络错误，请确保能够访问GitHub!\n')
        return None

    soup = BeautifulSoup(html, 'lxml')
    newUrl = ''
    try:
        div = soup.find(
            'div', class_='d-flex flex-items-center')
        string = div.find('a')['data-hydro-click'] + ''
        newUrl = (json.loads(string))['payload']['originating_url']
    except:
        print('获取重定向网址失败!\n')
        return None
    # 开始重定向到新网址
    print('重定向到：' + newUrl + '\n')
    html = get_html(newUrl)
    if html == None:
        print('网络错误，请确保能够访问GitHub!\n')
        return None
    soup = BeautifulSoup(html, 'lxml')

    try:
        release = soup.find('div', class_='border-top')
        posts['ver'] = release.find(
            'a', class_='muted-link css-truncate')['title']
        print('最新版本：' + posts['ver'])

        posts['log'] = release.find(
            'div', class_='markdown-body').find('p').text
        print('更新日志：' + posts['log'])

        posts['time'] = release.find('relative-time').text
        print('更新时间：' + posts['time'])

        posts['URL'] = 'https://github.com/' + release.find(
            'a', class_='d-flex flex-items-center min-width-0')['href']
        print('下载地址：' + posts['URL'] + '\n')
        print('若速度过慢可手动前往码云下载：\n\thttps://gitee.com/JasonMa233/Sakura_DesktopMascot/releases\n')
    except:
        print('Github数据爬取失败!\n')
        return None
    return posts


# 下面的go是自定义的回调函数，每次调用它都会打印出当前下载进度
def go(blocknum, blocksize, totalsize):
    if totalsize > 0:
        percent = blocknum*blocksize/totalsize
        if percent > 1:
            percent = 1
    else:
        percent = totalsize
    # blocknum是数据块的数量，我只下载一个压缩包，所以它等于1；blocksize是已经下载的文件大小，totalsize是文件总大小。
    print('\r已下载：{:.2%} （{:.3f}/{:.1f}MB）'.format(
        percent, blocknum * blocksize / 1000000, totalsize / 1000000), end='')


out = {0: '>按回车退出<\n\n', 1: '>按回车重试<\n\n', 2: '>按回车开始更新<\n\n'}


def Output(code):
    input(out.get(code, '无此消息!\n'))


# 终端命令：PyInstaller -F Update.py 打包py为exe
if __name__ == "__main__":
    print('\t-==八重樱桌宠==-\n\n')
    exePath, ver = 'Sakura_DesktopMascot.exe', '1'
    if len(sys.argv) > 1:
        os.chdir(sys.argv[1])
    if os.path.exists('Ver.data'):
        f = open('Ver.data')
        ver = f.readline()
        exePath = f.readline()
        f.close()
    else:
        print('请先运行桌宠！\n')
    if ver != '1':
        print('当前版本：' + ver + '\n')
    else:
        print('当前版本：未知  请尝试使用桌宠程序检查更新，或选择继续更新！\n')

    while True:
        print('检查更新中……\n\n')
        githubData = get_GithubData(
            'https://github.com/Jason-Ma-233/Sakura_DesktopMascot/releases/latest')
        if githubData != None:
            # 若拿到更新数据则检查是否需要更新
            if float(ver) < float(githubData['ver']):
                Output(2)
                # 下载压缩包
                tempPath = 'temp'
                if not os.path.exists(tempPath):
                    os.mkdir(tempPath)
                tempFilePath = 'temp/Update.zip'
                urlretrieve(githubData['URL'], tempFilePath, go)
                # 关闭程序
                print('关闭程序并解压中……\n')
                os.system("taskkill /F /IM " + exePath)
                # 解压覆盖
                f = zipfile.ZipFile(tempFilePath, 'r')
                for file in f.namelist():
                    if file != 'Update.exe':
                        f.extract(file, r'.')
                # 删除temp文件夹
                f.close()
                os.remove(tempFilePath)
                os.rmdir(tempPath)
                # 重启程序
                os.startfile(exePath)
                print('更新完成！\n\n3秒后自动关闭……\n')
                time.sleep(3)
                break
            else:
                print('无需更新！\n\n3秒后自动关闭……\n')
                time.sleep(3)
                break
        else:
            Output(1)
            continue
